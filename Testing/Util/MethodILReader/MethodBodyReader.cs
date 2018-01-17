/*
 * Code taken from Codeproject http://codeproject.com/csharp/sdilreader.asp
 * Parsing the IL of a Method Body By Sorin Serban
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Testing.Util.MethodILReader
{
	/// <summary>
	/// Object used to interpret a methods IL code.
	/// </summary>
	public class MethodBodyReader : IEnumerable<ILInstruction>
	{

		#region Fields

		private List<ILInstruction> _instructions = null;
		private MethodInfo _method = null;

		#endregion

		/// <summary>
		/// MethodBodyReader constructor
		/// </summary>
		/// <param name="mi">
		/// The System.Reflection defined MethodInfo
		/// </param>
		public MethodBodyReader( MethodInfo mi )
		{
			if( mi.GetMethodBody() != null )
			{
				_method = mi;
				_instructions = new List<ILInstruction>();
				ConstructInstructions();
			}
		}

		/// <summary>
		/// Gets a read-only list of the interpreted instructions in the method.
		/// </summary>
		public ILInstruction[] Instructions
		{
			get { return _instructions.ToArray(); }
		}

		#region Static il read methods

		/// <summary>
		/// Reads an Int16.
		/// </summary>
		/// <param name="il">An array of il instruction bytes.</param>
		/// <param name="position">Start index.</param>
		private static int ReadInt16( byte[] il, ref int position )
		{
			return ((il[position++] | (il[position++] << 8)));
		}

		/// <summary>
		/// Reads a UInt16.
		/// </summary>
		/// <param name="il">An array of il instruction bytes.</param>
		/// <param name="position">Start index.</param>
		private static ushort ReadUInt16( byte[] il, ref int position )
		{
			return (ushort)((il[position++] | (il[position++] << 8)));
		}

		/// <summary>
		/// Reads an Int32.
		/// </summary>
		/// <param name="il">An array of il instruction bytes.</param>
		/// <param name="position">Start index.</param>
		private static int ReadInt32( byte[] il, ref int position )
		{
			return (((il[position++] | (il[position++] << 8)) | (il[position++] << 0x10)) | (il[position++] << 0x18));
		}

		/// <summary>
		/// Reads an Int64.
		/// </summary>
		/// <param name="il">An array of il instruction bytes.</param>
		/// <param name="position">Start index.</param>
		private static ulong ReadInt64( byte[] il, ref int position )
		{
			return (ulong)(((il[position++] | (il[position++] << 8)) | (il[position++] << 0x10)) | (il[position++] << 0x18) | (il[position++] << 0x20) | (il[position++] << 0x28) | (il[position++] << 0x30) | (il[position++] << 0x38));
		}

		/// <summary>
		/// Reads a Double.
		/// </summary>
		/// <param name="il">An array of il instruction bytes.</param>
		/// <param name="position">Start index.</param>
		private static double ReadDouble( byte[] il, ref int position )
		{
			return (((il[position++] | (il[position++] << 8)) | (il[position++] << 0x10)) | (il[position++] << 0x18) | (il[position++] << 0x20) | (il[position++] << 0x28) | (il[position++] << 0x30) | (il[position++] << 0x38));
		}

		/// <summary>
		/// Reads a SByte.
		/// </summary>
		/// <param name="il">An array of il instruction bytes.</param>
		/// <param name="position">Start index.</param>
		private static sbyte ReadSByte( byte[] il, ref int position )
		{
			return (sbyte)il[position++];
		}

		/// <summary>
		/// Reads a Byte.
		/// </summary>
		/// <param name="il">An array of il instruction bytes.</param>
		/// <param name="position">Start index.</param>
		private static byte ReadByte( byte[] il, ref int position )
		{
			return (byte)il[position++];
		}

		/// <summary>
		/// Reads a Single.
		/// </summary>
		/// <param name="il">An array of il instruction bytes.</param>
		/// <param name="position">Start index.</param>
		private static Single ReadSingle( byte[] il, ref int position )
		{
			return (Single)(((il[position++] | (il[position++] << 8)) | (il[position++] << 0x10)) | (il[position++] << 0x18));
		}

		#endregion

		#region Methods

		/// <summary>
		/// Constructs the array of ILInstructions according to the IL byte code.
		/// </summary>
		private void ConstructInstructions()
		{
			byte[] il = _method.GetMethodBody().GetILAsByteArray();
			Module module = _method.Module;
			int position = 0;
			while( position < il.Length )
			{
				ILInstruction instruction = new ILInstruction();

				// get the operation code of the current instruction
				OpCode code = OpCodes.Nop;
				ushort value = il[position++];
				if( value != 0xfe )
				{
					code = OpCodeHelper.SingleByteOpCodes[(int)value];
				}
				else
				{
					value = il[position++];
					code = OpCodeHelper.MultiByteOpCodes[(int)value];
					value = (ushort)(value | 0xfe00);
				}
				instruction.Code = code;
				instruction.Offset = position - 1;
				int metadataToken = 0;
				// get the operand of the current operation
				switch( code.OperandType )
				{
					case OperandType.InlineBrTarget:
						metadataToken = ReadInt32( il, ref position );
						metadataToken += position;
						instruction.Operand = metadataToken;
						break;
					case OperandType.InlineField:
						metadataToken = ReadInt32( il, ref position );
						instruction.Operand = module.ResolveField( metadataToken );
						break;
					case OperandType.InlineMethod:
						metadataToken = ReadInt32( il, ref position );
						try
						{
							instruction.Operand = module.ResolveMethod( metadataToken );
						}
						catch
						{
							instruction.Operand = module.ResolveMember( metadataToken );
						}
						break;
					case OperandType.InlineSig:
						metadataToken = ReadInt32( il, ref position );
						instruction.Operand = module.ResolveSignature( metadataToken );
						break;
					case OperandType.InlineTok:
						metadataToken = ReadInt32( il, ref position );
						instruction.Operand = module.ResolveType( metadataToken );
						// TODO InlineTok operand can also be MethodInfo or FieldInfo.
						break;
					case OperandType.InlineType:
						metadataToken = ReadInt32( il, ref position );
						instruction.Operand = module.ResolveType( metadataToken );
						break;
					case OperandType.InlineI:
						{
							instruction.Operand = ReadInt32( il, ref position );
							break;
						}
					case OperandType.InlineI8:
						{
							instruction.Operand = ReadInt64( il, ref position );
							break;
						}
					case OperandType.InlineNone:
						{
							instruction.Operand = null;
							break;
						}
					case OperandType.InlineR:
						{
							instruction.Operand = ReadDouble( il, ref position );
							break;
						}
					case OperandType.InlineString:
						{
							metadataToken = ReadInt32( il, ref position );
							instruction.Operand = module.ResolveString( metadataToken );
							break;
						}
					case OperandType.InlineSwitch:
						{
							int count = ReadInt32( il, ref position );
							int[] casesAddresses = new int[count];
							for( int i = 0; i < count; i++ )
							{
								casesAddresses[i] = ReadInt32( il, ref position );
							}
							int[] cases = new int[count];
							for( int i = 0; i < count; i++ )
							{
								cases[i] = position + casesAddresses[i];
							}
							break;
						}
					case OperandType.InlineVar:
						{
							instruction.Operand = ReadUInt16( il, ref position );
							break;
						}
					case OperandType.ShortInlineBrTarget:
						{
							instruction.Operand = ReadSByte( il, ref position ) + position;
							break;
						}
					case OperandType.ShortInlineI:
						{
							instruction.Operand = ReadSByte( il, ref position );
							break;
						}
					case OperandType.ShortInlineR:
						{
							instruction.Operand = ReadSingle( il, ref position );
							break;
						}
					case OperandType.ShortInlineVar:
						{
							instruction.Operand = ReadByte( il, ref position );
							break;
						}
					default:
						{
							//TODO Do Not Raise Reserved Exception Types
							throw new Exception( "Unknown operand type." );
						}
				}
				_instructions.Add( instruction );
			}
		}

		/// <summary>
		/// Gets the IL code of the method
		/// </summary>
		public string GetBodyCode()
		{
			StringBuilder result = new StringBuilder();
			foreach( ILInstruction instruction in _instructions )
			{
				result.AppendLine( instruction.GetCode() );
			}
			return result.ToString();

		}

		/// <summary>
		/// Get a list of all the locally assigned variable types.
		/// </summary>
		public List<Type> GetLocalTypes()
		{
			ILInstruction previous = null;
			List<Type> locals = new List<Type>();
			foreach( ILInstruction instruction in _instructions )
			{
				if( (instruction.Code == OpCodes.Stloc || instruction.Code == OpCodes.Stloc_S ||
					instruction.Code == OpCodes.Stloc_0 || instruction.Code == OpCodes.Stloc_1 ||
					instruction.Code == OpCodes.Stloc_2 || instruction.Code == OpCodes.Stloc_3) && previous != null )
				{
					ConstructorInfo ci = previous.Operand as ConstructorInfo;
					if( ci != null )
					{
						locals.Add( ci.DeclaringType );
						continue;
					}

					if( previous.Code == OpCodes.Ldstr )
					{
						locals.Add( typeof( string ) );
					}
					else if( previous.Code == OpCodes.Ldc_I4_0 || previous.Code == OpCodes.Ldc_I4_1 ||
						previous.Code == OpCodes.Ldc_I4_2 || previous.Code == OpCodes.Ldc_I4_3 ||
						previous.Code == OpCodes.Ldc_I4_4 || previous.Code == OpCodes.Ldc_I4_5 ||
						previous.Code == OpCodes.Ldc_I4_6 || previous.Code == OpCodes.Ldc_I4_7 ||
						previous.Code == OpCodes.Ldc_I4_8 || previous.Code == OpCodes.Ldc_I4_M1 ||
						previous.Code == OpCodes.Ldc_I4_S )
					{
						locals.Add( typeof( int ) );
					}
					else if( previous.Code == OpCodes.Ldc_I8 )
					{
						locals.Add( typeof( long ) );
					}
					else if( previous.Code == OpCodes.Ldc_R4 || previous.Code == OpCodes.Ldc_R8 )
					{
						locals.Add( typeof( float ) );
					}
				}
				previous = instruction;
			}
			return locals;
		}

		/// <summary>
		/// Get the first free local variable index.
		/// </summary>
		public ushort GetFreeLocalIndex()
		{
			ushort freeLocalIndex = 0;
			foreach( ILInstruction instruction in _instructions )
			{
				if( instruction.Code == OpCodes.Stloc_0 && freeLocalIndex <= 0 )
				{
					freeLocalIndex = 1;
				}
				else if( instruction.Code == OpCodes.Stloc_1 && freeLocalIndex <= 1 )
				{
					freeLocalIndex = 2;
				}
				else if( instruction.Code == OpCodes.Stloc_2 && freeLocalIndex <= 2 )
				{
					freeLocalIndex = 3;
				}
				else if( instruction.Code == OpCodes.Stloc_3 && freeLocalIndex <= 3 )
				{
					freeLocalIndex = 4;
				}
				else if( (instruction.Code == OpCodes.Stloc || instruction.Code == OpCodes.Stloc_S)
					  && freeLocalIndex <= (byte)instruction.Operand )
				{
					freeLocalIndex = (ushort)((byte)instruction.Operand + 1);
				}
			}
			return freeLocalIndex;
		}

		private int GetStackDeltaPopBehaviour( ILInstruction instruction )
		{
			int stackDelta = 0;
			switch( instruction.Code.StackBehaviourPop )
			{
				case StackBehaviour.Pop1:
				case StackBehaviour.Popi:
				case StackBehaviour.Popref:
					stackDelta = -1;
					break;
				case StackBehaviour.Pop1_pop1:
				case StackBehaviour.Popi_pop1:
				case StackBehaviour.Popi_popi:
				case StackBehaviour.Popi_popi8:
				case StackBehaviour.Popi_popr4:
				case StackBehaviour.Popi_popr8:
				case StackBehaviour.Popref_pop1:
				case StackBehaviour.Popref_popi:
					stackDelta = -2;
					break;
				case StackBehaviour.Popi_popi_popi:
				case StackBehaviour.Popref_popi_pop1:
				case StackBehaviour.Popref_popi_popi:
				case StackBehaviour.Popref_popi_popi8:
				case StackBehaviour.Popref_popi_popr4:
				case StackBehaviour.Popref_popi_popr8:
				case StackBehaviour.Popref_popi_popref:
					stackDelta = -3;
					break;
				case StackBehaviour.Varpop:
					MethodBase mb = instruction.Operand as MethodBase;
					if( mb != null )
					{
						stackDelta -= mb.GetParameters().Length;
						if( !mb.IsStatic && !mb.IsConstructor )
						{
							stackDelta -= 1;
						}
					}
					else if( instruction.Code != OpCodes.Ret )
					{
						stackDelta = -1;
					}
					break;
				case StackBehaviour.Pop0:
				default:
					break;
			}
			return stackDelta;
		}

		private int GetStackDeltaPushBehaviour( ILInstruction instruction )
		{
			int stackDelta = 0;
			switch( instruction.Code.StackBehaviourPush )
			{
				case StackBehaviour.Push1:
				case StackBehaviour.Pushi:
				case StackBehaviour.Pushi8:
				case StackBehaviour.Pushr4:
				case StackBehaviour.Pushr8:
				case StackBehaviour.Pushref:
					stackDelta = 1;
					break;
				case StackBehaviour.Push1_push1:
					stackDelta = 2;
					break;
				case StackBehaviour.Varpush:
					MethodInfo mi = instruction.Operand as MethodInfo;
					if( mi != null && !(mi.ReturnParameter.ParameterType.Equals( typeof( void ) )) )
					{
						stackDelta = 1;
					}
					break;
				case StackBehaviour.Push0:
				default:
					break;
			}
			return stackDelta;
		}

		/// <summary>
		/// Calculates the effect an instruction will have on the evaluation stack.
		/// </summary>
		public int GetStackDelta( ILInstruction instruction )
		{
			int stackDelta = 0;
			stackDelta += GetStackDeltaPopBehaviour( instruction );
			stackDelta += GetStackDeltaPushBehaviour( instruction );
			return stackDelta;
		}

		#endregion


		#region IEnumerable<ILInstruction> Members

		/// <summary>
		/// Returns an enumerator that iterates through the list of instructions.
		/// </summary>
		public IEnumerator<ILInstruction> GetEnumerator()
		{
			return _instructions.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _instructions.GetEnumerator();
		}

		#endregion
	}
}
