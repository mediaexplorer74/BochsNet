using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Definitions.Enumerations;

namespace CPU.Event_Arguments
{
    public class InstructionEventArgument: Core.Monitor.EventArgument
    {

        
        #region "Enumerations"
        
        #endregion 

        #region "Attributes"

        protected Instructions.Instruction mInstruction;
        protected InstructionType mInstructionType;
        #endregion 


        #region Properties"
        public InstructionType InstructionType
        {
            get
            {
                return mInstructionType;
            }
        }

        public Instructions.Instruction Instruction
        {
            get
            {
                return mInstruction;
            }
        }
        #endregion 



        #region "Constructors"

        protected InstructionEventArgument()
        {
        }

      
        public InstructionEventArgument(string Description,Instructions.Instruction oInstruction,InstructionType oInstructionType):base (Description)
        {
            mInstruction = oInstruction;
            mInstructionType = oInstructionType;
        }



        #endregion 
    }
}
