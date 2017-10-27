namespace OrchardNorthwind.Data.Scenarios
{
	public abstract class ScenarioBase
	{
        /// <summary>
        /// 
        /// </summary>
        public bool IsRunning = false;
        public abstract void Run();
	}
}