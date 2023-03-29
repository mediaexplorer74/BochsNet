CPUInfo
Please, add 'Closing' event handler, which will cancel closing and call 'Hide()' instead.
Proposed
#319 | Created 2011-10-13 | Updated 2013-02-13
RETnear16 Issue
CPU executes wrong instructions after return.
it executes the next instruction in Pool after Ret.
CommitTrace is updated to :

public void CommitTrace(uint InstructionLength)
{
mPIndex = mPIndex_Start+ InstructionLength;
}

the issue was in mPIndex points wrongly after the pool so the execution loop is not ended after Ret and tries to get the next -non existed instruction - from pool.
2011-08-19
2013-02-13
2013-05-14
2013-05-14
2013-06-13
Resolved
#147 | Created 2011-08-19 | Updated 2013-06-13
Instruction Address Displayed Wrongly
Instruction address points to the next instruction instead of the current executed one.
Resolved
#92 | Created 2011-07-30 | Updated 2013-06-13
Call Instruction
implementing call instruction.
I will implement the minimum requirement to run calls for bios calls.
call instruction is working now.
I had to implement Push instruction also.
2011-07-31
2013-02-13
2013-05-14
2013-05-14
2013-06-13
Resolved
#91 | Created 2011-07-30 | Updated 2013-06-13
Write Physical Page
to be implemented
WritePhysicalPage is working now ... still not all paths but I had to fix an create many functions around it.
better status now.
2011-07-29
2013-02-13
2017-12-09
Active
#88 | Created 2011-07-25 | Updated 2017-12-09