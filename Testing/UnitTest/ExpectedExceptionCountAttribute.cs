using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Testing.Util.MethodILReader;

using Testing.TestRunner;

using Testing.Util.SymbolStore;

using System.Diagnostics.SymbolStore;
using System.IO;

namespace Testing.UnitTest
{


	/// <summary>
	/// This test runner will break a test method into variable assignments and method calls.
	/// Each code segment is executed within a try block. The exceptions throw are counted and
	/// compared the the expected type to determine a pass or fail.
	/// </summary>
	/// <remarks>
	///	Injects IL instructions to convert the test method from:
	///	<code>
	///	[Test]
	///	[ExpectedExceptionCount( typeof( [ExceptionType] ) )]
	///	public void [MethodName]()
	///	{
	///			[FirstCall]
	///			[SecondCall]
	///	}
	///	</code>
	///	to:
	///	<code>
	///	public List&lt;Exception&gt; [MethodName]_TestMethod()
	///	{
	///			List&lt;Exception&gt; exceptions = new List&lt;Exception&gt;();
	///			try
	///			{
	///				[FirstCall]
	///				exceptions.Add( null );
	///			}
	///			catch( Exception exp )
	///			{
	///				exceptions.Add( exp );
	///			}
	///			try
	///			{
	///				[SecondCall]
	///				exceptions.Add( null );
	///			}
	///			catch( Exception exp )
	///			{
	///				exceptions.Add( exp );
	///			}
	///			return exceptions;
	///	}
	/// </code>
	/// </remarks>
	[AttributeUsage( AttributeTargets.Method )]
	public class ExpectedExceptionCountAttribute : Attribute, ITestRunner
	{

		#region Private Fields

		private const int UseTestCount = -1;
		private const bool DefaultFailOnOtherExceptions = true;
		const string ExpectedExceptionCountMessage = "Expected Exception: {0} was thrown in {1} out of {2} cases, expected {3}. {4} other exception{5} thrown.\n";

		private Type _exceptionType;
		private int _exceptionCount;
		private bool _failOnOtherExceptions;

		private delegate Dictionary<int, Exception> TestDelegate();

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new ExpectedExceptionCountAttribute.
		/// </summary>
		/// <param name="exceptionType">The type of exception that is expected to be thrown.</param>
		/// <param name="exceptionCount">The number of expected exceptions.</param>
		/// <param name="failOnOtherExpections">Indicates whether the test should fail if other exceptions are caught.</param>
		public ExpectedExceptionCountAttribute( Type exceptionType, int exceptionCount, bool failOnOtherExpections )
		{
			if( exceptionType == null )
			{
				throw new ArgumentNullException( "exceptionType is null.", "exceptionType" );
			}
			_exceptionType = exceptionType;
			_exceptionCount = exceptionCount;
			_failOnOtherExceptions = failOnOtherExpections;
		}

		/// <summary>
		/// Creates a new ExpectedExceptionCountAttribute.
		/// </summary>
		/// <param name="exceptionTypeName">The name of the type of exception that is expected to be thrown.</param>
		/// <param name="exceptionCount">The number of expected exceptions.</param>
		/// <param name="failOnOtherExceptions">Indicates whether the test should fail if other exceptions are caught.</param>
		public ExpectedExceptionCountAttribute( string exceptionTypeName, int exceptionCount, bool failOnOtherExceptions )
			: this( Type.GetType( exceptionTypeName ), exceptionCount, failOnOtherExceptions ) { }

		/// <summary>
		/// Creates a new ExpectedExceptionCountAttribute.
		/// </summary>
		/// <param name="exceptionType">The type of exception that is expected to be thrown.</param>
		/// <param name="exceptionCount">The number of expected exceptions.</param>
		public ExpectedExceptionCountAttribute( Type exceptionType, int exceptionCount )
			: this( exceptionType, exceptionCount, DefaultFailOnOtherExceptions ) { }

		/// <summary>
		/// Creates a new ExpectedExceptionCountAttribute.
		/// </summary>
		/// <param name="exceptionTypeName">The name of the type of exception that is expected to be thrown.</param>
		/// <param name="exceptionCount">The number of expected exceptions.</param>
		public ExpectedExceptionCountAttribute( string exceptionTypeName, int exceptionCount )
			: this( exceptionTypeName, exceptionCount, DefaultFailOnOtherExceptions ) { }

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the type of exception that is expected to be thrown.
		/// </summary>
		public Type ExceptionType
		{
			get { return _exceptionType; }
			set
			{
				if( value == null )
				{
					throw new ArgumentNullException( "exceptionType is null.", "exceptionType" );
				}
				_exceptionType = value;
			}
		}

		/// <summary>
		/// Gets or sets the expected number of exceptions.
		/// </summary>
		public int ExceptionCount
		{
			get { return _exceptionCount; }
			set { _exceptionCount = value; }
		}

		/// <summary>
		/// Indicates whether the test should fail if an exception of an unexpected type is thrown.
		/// </summary>
		public bool FailOnOtherExceptions
		{
			get { return _failOnOtherExceptions; }
			set { _failOnOtherExceptions = value; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Break the method instructions in to section based on 'Pop' and 'Stloc' instructions.
		/// </summary>
		private List<List<ILInstruction>> GetMethodSections( MethodBodyReader reader )
		{
			List<List<ILInstruction>> sections = new List<List<ILInstruction>>();
			List<ILInstruction> section = new List<ILInstruction>();
			ILInstruction previous = null;
			int stack = 0;
			foreach( ILInstruction instruction in reader )
			{
				stack += reader.GetStackDelta( instruction );
				// The return statement is replaced later, so don't add it here.
				if( instruction.Code != OpCodes.Ret )
				{
					section.Add( instruction );
				}
				if( IsSectionEnd( instruction, previous ) && stack == 0 )
				{
					sections.Add( section );
					section = new List<ILInstruction>();
				}
				previous = instruction;
			}
			if( section.Count > 0 )
			{
				sections.Add( section );
			}
			return sections;
		}

		/// <summary>
		/// Gets a value indicating whether the instruction pair represents a logical section break;
		/// </summary>
		private static bool IsSectionEnd( ILInstruction instruction, ILInstruction previous )
		{
			if( instruction.Code == OpCodes.Pop )
			{
				return true;
			}
			else
			{
				if( instruction.Code == OpCodes.Stloc || instruction.Code == OpCodes.Stloc_0 ||
								instruction.Code == OpCodes.Stloc_1 || instruction.Code == OpCodes.Stloc_2 ||
								instruction.Code == OpCodes.Stloc_3 || instruction.Code == OpCodes.Stloc_S )
				{
					return true;
				}
				else
				{
					if( previous != null )
					{
						if( (previous.Code == OpCodes.Call || previous.Code == OpCodes.Calli ||
							previous.Code == OpCodes.Callvirt) && instruction.Code == OpCodes.Nop )
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Declare the local variables used in the new method.
		/// </summary>
		private void DeclareLocalVariableTypes( ILGenerator generator, MethodBodyReader reader )
		{
			List<Type> locals = reader.GetLocalTypes();
			// Declare all the local variable types.
			foreach( Type type in locals )
			{
				generator.DeclareLocal( type );
			}
		}

		/// <summary>
		/// Emit the required IL instructions to the new method.
		/// </summary>
		private DynamicMethod BuildTestMethod( Type fixtureType, string methodName, MethodBodyReader reader )
		{
			Type exceptionType = typeof( Exception );
			Type exceptionCollectionType = typeof( Dictionary<int, Exception> );
			ConstructorInfo exceptionCollectionTypeConstructor = exceptionCollectionType.GetConstructor( Type.EmptyTypes );
			MethodInfo exceptionCollectionTypeAddMethod = exceptionCollectionType.GetMethod( "Add" );

			List<List<ILInstruction>> sections = GetMethodSections( reader );
			ushort freeLocalIndex = reader.GetFreeLocalIndex();
			ushort localExceptionCollection = freeLocalIndex++;
			ushort localException = freeLocalIndex++;

			DynamicMethod testMethod = new DynamicMethod( methodName, exceptionCollectionType,
				new Type[] { fixtureType }, fixtureType );
			ILGenerator generator = testMethod.GetILGenerator();
			DeclareLocalVariableTypes( generator, reader );

			// Instantiate exception collection.
			generator.DeclareLocal( exceptionCollectionType );
			generator.Emit( OpCodes.Nop );
			generator.Emit( OpCodes.Newobj, exceptionCollectionTypeConstructor );
			generator.Emit( OpCodes.Stloc_S, localExceptionCollection );

			foreach( List<ILInstruction> sect in sections )
			{
				// Start the try block.
				generator.BeginExceptionBlock();
				foreach( ILInstruction instruction in sect )
				{
					instruction.EmitTo( generator );
				}
				// If no exception was thrown add a null reference to the collection.
				generator.Emit( OpCodes.Ldloc_S, localExceptionCollection );
				generator.Emit( OpCodes.Ldc_I4, sect[0].Offset );
				generator.Emit( OpCodes.Ldnull );
				generator.Emit( OpCodes.Callvirt, exceptionCollectionTypeAddMethod );
				generator.Emit( OpCodes.Nop );
				// Catch Exception.
				generator.BeginCatchBlock( exceptionType );
				// Save thrown exception.
				generator.DeclareLocal( exceptionType );
				generator.Emit( OpCodes.Stloc_S, localException );
				generator.Emit( OpCodes.Nop );
				// Add the exception to the collection.
				generator.Emit( OpCodes.Ldloc_S, localExceptionCollection );
				generator.Emit( OpCodes.Ldc_I4, sect[0].Offset );
				generator.Emit( OpCodes.Ldloc_S, localException );
				generator.Emit( OpCodes.Callvirt, exceptionCollectionTypeAddMethod );
				generator.Emit( OpCodes.Nop );
				// Exit the try block.
				generator.EndExceptionBlock();
			}
			generator.Emit( OpCodes.Ldloc_S, localExceptionCollection );
			generator.Emit( OpCodes.Ret );

			return testMethod;
		}

		/// <summary>
		/// Generate new dynamic method.
		/// </summary>
		private DynamicMethod GetTestMethod( Type fixtureType, MethodInfo method )
		{
			MethodBodyReader r = new MethodBodyReader( method );

			DynamicMethod testMethod = BuildTestMethod( fixtureType, String.Concat( method.Name, "_TestMethod" ), r );

			return testMethod;
		}

		#endregion

		#region ITestRunner Members

		/// <summary>
		/// Run the test.
		/// </summary>
		public void RunTest( ITest test )
		{
			Type fixtureType = test.Fixture.Instance.GetType();
			MethodInfo method = test.TestMethod;

			DynamicMethod testMethod = GetTestMethod( fixtureType, method );

			TestTimer timer = new TestTimer();
			Dictionary<int, Exception> exceptionsThrown;
			TestDelegate testDelegate = testMethod.CreateDelegate( typeof( TestDelegate ), test.Fixture.Instance ) as TestDelegate;
			try
			{
				timer.Start( test );
				exceptionsThrown = testDelegate();
				timer.Stop();
			}
			catch( InvalidProgramException exp )
			{
				timer.Stop();
				throw new InvalidOperationException( "Unable to catch thrown exceptions in test method.", exp );
			}
			finally
			{
				timer.Stop();
			}

			SequenceManager sm = new SequenceManager( test.TestMethod );
			int expectedExceptionCount = 0;
			int unexpectedExpectedCount = 0;
			int i = 0;
			foreach( KeyValuePair<int, Exception> exception in exceptionsThrown )
			{
				if( exception.Value != null )
				{
					Type thrownExceptionType = exception.Value.GetType();
					test.Result.Output.AppendFormat( "[{0}] Expected Exception: {1}", i + 1, ExceptionType.FullName );
					if( thrownExceptionType.Equals( ExceptionType ) )
					{
						expectedExceptionCount++;
						test.Result.Output.AppendLine( " was thrown." );
					}
					else
					{
						// Get line offset from il instruction offset.
						unexpectedExpectedCount++;
						test.Result.Output.AppendLine( " was NOT thrown." );
						test.Result.Output.Append( "\tThrown Exception Was: " );
						if( sm.IsSourceAvailable() )
						{
							int sequenceOffset = Math.Abs( Array.BinarySearch<int>( sm.Offsets, exception.Key ) );
							string dynamicLine = String.Format( "   at {0}_TestMethod({1} )",
								test.TestMethod.Name, test.Fixture.FixtureType.Name );
							string sourceLine = sm.GetStackTrace( sequenceOffset );
							test.Result.Output.AppendLine(
								exception.Value.ToString().Replace( dynamicLine, sourceLine ) );
							if( test.Result.StackTrace == null )
							{
								test.Result.SetFilteredStackTrace(
									exception.Value.StackTrace.Replace( dynamicLine, sourceLine ) );
							}
						}
						else
						{
							test.Result.Output.AppendLine( exception.Value.ToString() );
							if( test.Result.StackTrace == null )
							{
								test.Result.SetFilteredStackTrace( exception.Value.StackTrace );
							}
						}
					}
				}
				else
				{
					test.Result.Output.AppendFormat( "[{0}] No exception was thrown.\n", i + 1 );
				}
				i++;
			}

			if( test.Result.StackTrace == null && sm.IsSourceAvailable() )
			{
				test.Result.StackTrace = sm.GetStackTrace( 0 );
			}

			int expectedNumberOfExceptions = (ExceptionCount == UseTestCount ? exceptionsThrown.Count : ExceptionCount);
			test.Result.Message.AppendFormat( ExpectedExceptionCountMessage,
				ExceptionType.FullName, expectedExceptionCount, exceptionsThrown.Count, expectedNumberOfExceptions,
				unexpectedExpectedCount, (unexpectedExpectedCount != 1 ? " was" : "s were") );
			if( expectedExceptionCount != expectedNumberOfExceptions || (FailOnOtherExceptions && unexpectedExpectedCount > 0) )
			{
				test.Result.Status = TestStatus.Fail;
			}
			else
			{
				test.Result.Status = TestStatus.Pass;
			}
		}

		#endregion
	}
}