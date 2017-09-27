using System.Threading;

namespace Framework 
{
    public class FThread
	{
	    private Thread m_thread;
	    private bool m_terminateFlag;
	    private object m_terminateFlagMutex;
	
	    public void Run()
	    {
	        m_thread.Start(this);
	    }
	
	    protected static void ThreadProc(object obj)
	    {
	        FThread me = (FThread)obj;
	        me.Main();
	    }
	
	    protected virtual void Main()
	    {
	
	    }
	
	    public void WaitTermination()
	    {
	        m_thread.Join();
	    }
	
	    public void SetTerminateFlag()
	    {
	        lock (m_terminateFlagMutex)
	        {
	            m_terminateFlag = true;
	        }
	    }
	
	    public bool IsTerminateFlagSet()
	    {
	        lock (m_terminateFlagMutex)
	        {
	            return m_terminateFlag;
	        }
	    }

        public void Interrupt()
        {
            m_thread.Interrupt();
        }

        public FThread()
	    {
	        m_thread = new Thread(ThreadProc);
	        m_terminateFlag = false;
	        m_terminateFlagMutex = new object();
	    }
	}
}
