using System;
using System.Collections.Generic;
using System.Text;

namespace Testing.UnitTest
{

	/// <summary>
	/// A callback method which will return a list of test arguments. This method will be called when the test is run.
	/// </summary>
	public delegate List<object[]> TestArgumentListCallback();

}
