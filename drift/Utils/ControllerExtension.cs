using Microsoft.AspNetCore.Mvc;

namespace drift.Utils
{
	public static class ControllerExtension
	{
		public static string GetCurrentControllerName(this Controller controller, string name)
		{
			return name.Replace("Controller", "");
		}
	}
}
