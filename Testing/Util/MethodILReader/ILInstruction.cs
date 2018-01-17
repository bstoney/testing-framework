/*
 * Code taken from Codeproject http://codeproject.com/csharp/sdilreader.asp
 * Parsing the IL of a Method Body By Sorin Serban
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;

namespace Testing.Util.MethodILReader
{
	/// <summary>
	/// A structure which represents an IL instruction.
	/// </summary>
	public sealed class ILInstruction
	{
		#region fields

		private OpCode code;
		private object operand;
		private byte[] operandData;
		private int offset;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the OpCode.
		/// </summary>
		public OpCode Code
		{
			get { return code; }
			set { code = value; }
		}

		/// <summary>
		/// Gets or sets the operand.
		/// </summary>
		public object Operand
		{
			get { return operand; }
			set { operand = value; }
		}

		/// <summary>
		/// Gets or sets additional operand data.
		/// </summary>
		public byte[] OperandData
		{
			get { return operandData; }
			set { operandData = value; }
		}

		/// <summary>
		/// Gets or sets the offset.
		/// </summary>
		public int Offset
		{
			get { return offset; }
			set { offset = value; }
		}

		#endregion

		/// <summary>
		/// Returns a friendly strign representation of this instruction
		/// </summary>
		public string GetCode()
		{
			string result = "";
			result += GetExpandedOffset( offset ) + " : " + code;
			if( operand != null )
			{
				switch( code.OperandType )
				{
					case OperandType.InlineField:
						System.Reflection.FieldInfo fOperand = ((System.Reflection.FieldInfo)operand);
						result += " " + OpCodeHelper.ProcessSpecialTypes( fOperand.FieldType.ToString() ) + " " +
							OpCodeHelper.ProcessSpecialTypes( fOperand.ReflectedType.ToString() ) +
							"::" + fOperand.Name + "";
						break;
					case OperandType.InlineMethod:
						try
						{
							System.Reflection.MethodInfo mOperand = (System.Reflection.MethodInfo)operand;
							result += " ";
							if( !mOperand.IsStatic ) result += "instance ";
							result += OpCodeHelper.ProcessSpecialTypes( mOperand.ReturnType.ToString() ) +
								" " + OpCodeHelper.ProcessSpecialTypes( mOperand.ReflectedType.ToString() ) +
								"::" + mOperand.Name + "()";
						}
						catch
						{
							try
							{
								System.Reflection.ConstructorInfo mOperand = (System.Reflection.ConstructorInfo)operand;
								result += " ";
								if( !mOperand.IsStatic ) result += "instance ";
								result += "void " +
									OpCodeHelper.ProcessSpecialTypes( mOperand.ReflectedType.ToString() ) +
									"::" + mOperand.Name + "()";
							}
							catch
							{
							}
						}
						break;
					case OperandType.ShortInlineBrTarget:
						result += " " + GetExpandedOffset( (int)operand );
						break;
					case OperandType.InlineType:
						result += " " + OpCodeHelper.ProcessSpecialTypes( operand.ToString() );
						break;
					case OperandType.InlineString:
						if( operand.ToString() == "\r\n" ) result += " \"\\r\\n\"";
						else result += " \"" + operand.ToString() + "\"";
						break;
					default: result += " not supported"; break;
				}
			}
			return result;

		}

		/// <summary>
		/// Add enough zeros to a number as to be represented on 4 characters
		/// </summary>
		/// <param name="offset">The number that must be represented on 4 characters.</param>
		private string GetExpandedOffset( int offset )
		{
			return offset.ToString( "0000" );
		}

		/// <summary>
		/// Emit the instruction to the supplied ILGenerator.
		/// </summary>
		public void EmitTo( ILGenerator generator )
		{
			switch( Code.OperandType )
			{
				case OperandType.InlineBrTarget:
					generator.Emit( Code, (int)Operand );
					break;
				case OperandType.InlineField:
					generator.Emit( Code, (FieldInfo)Operand );
					break;
				case OperandType.InlineI:
					generator.Emit( Code, (int)Operand );
					break;
				case OperandType.InlineI8:
					generator.Emit( Code, (long)Operand );
					break;
				case OperandType.InlineMethod:
					if( Operand is ConstructorInfo )
					{
						generator.Emit( Code, (ConstructorInfo)Operand );
					}
					else if( Operand is System.Reflection.MemberInfo )
					{
						generator.Emit( Code, (MethodInfo)Operand );
					}
					else
					{
						throw new NotImplementedException();
					}
					break;
				case OperandType.InlineNone:
					generator.Emit( Code );
					break;
				case OperandType.InlineR:
					generator.Emit( Code, (double)Operand );
					break;
				case OperandType.InlineSig:
					throw new NotImplementedException( Code.ToString() );
				case OperandType.InlineString:
					generator.Emit( Code, (string)Operand );
					break;
				case OperandType.InlineSwitch:
					throw new NotImplementedException( Code.ToString() );
				case OperandType.InlineTok:
					generator.Emit( Code, (Type)Operand );
					// TODO InlineTok operand can also be MethodInfo or FieldInfo.
					break;
				case OperandType.InlineType:
					generator.Emit( Code, (Type)Operand );
					break;
				case OperandType.InlineVar:
					generator.Emit( Code, (ushort)Operand );
					break;
				case OperandType.ShortInlineBrTarget:
					generator.Emit( Code, (int)Operand );
					break;
				case OperandType.ShortInlineI:
					generator.Emit( Code, (sbyte)Operand );
					break;
				case OperandType.ShortInlineR:
					generator.Emit( Code, (float)Operand );
					break;
				case OperandType.ShortInlineVar:
					generator.Emit( Code, (byte)Operand );
					break;
				default:
					throw new NotImplementedException( Code.ToString() );
			}
		}
	}
}
