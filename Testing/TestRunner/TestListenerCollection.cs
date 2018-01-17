// Project: Testing, File: RunnableBase.cs
// Namespace: Testing.TestRunner, Class: RunnableBase
// Path: D:\Development\Library.2005\Testing\TestRunner, Author: bstoney
// Code lines: 111, Size of file: 2.76 KB
// Creation date: 26/06/2006 8:19 AM
// Last modified: 29/06/2006 3:51 PM
// Copyright Plan B Financial Services

using System;
using System.Collections.Generic;
using System.Collections;

namespace Testing.TestRunner
{
	/// <summary>
	/// A class which provides the basic implementation of IRunnable.
	/// </summary>
	internal class TestListenerCollection : CollectionBase
	{
		/// <summary>
		/// Add a test listener to be notified.
		/// </summary>
		public void Add( ITestListener listener )
		{
			if( !InnerList.Contains( listener ) )
			{
				InnerList.Add( listener );
			}
		}

		/// <summary>
		/// Remove a test listener from the notification list.
		/// </summary>
		public void Remove( ITestListener listener )
		{
			InnerList.Remove( listener );
		}

		#region Notify Listeners

		/// <summary>
		/// Notifies the listeners at the start of a test suite.
		/// </summary>
		public void NotifyBeginTestSuite( ITestSuite testSuite )
		{
			foreach( ITestListener listener in InnerList )
			{
				listener.BeginTestSuite( testSuite );
			}
		}
		/// <summary>
		/// Notifies the listeners at the end of a test suite.
		/// </summary>
		public void NotifyEndTestSuite( ITestSuite testSuite )
		{
			foreach( ITestListener listener in InnerList )
			{
				listener.EndTestSuite( testSuite );
			}
		}
		/// <summary>
		/// Notifies the listeners at the start of a fixture.
		/// </summary>
		public void NotifyBeginFixture( IFixture fixture )
		{
			foreach( ITestListener listener in InnerList )
			{
				listener.BeginFixture( fixture );
			}
		}
		/// <summary>
		/// Notifies the listeners at the end of a fixture.
		/// </summary>
		public void NotifyEndFixture( IFixture fixture )
		{
			foreach( ITestListener listener in InnerList )
			{
				listener.EndFixture( fixture );
			}
		}
		/// <summary>
		/// Notifies the listeners at the start of a test.
		/// </summary>
		public void NotifyBeginTest( ITest test )
		{
			foreach( ITestListener listener in InnerList )
			{
				listener.BeginTest( test );
			}
		}
		/// <summary>
		/// Notifies the listeners at the end of a test.
		/// </summary>
		public void NotifyEndTest( ITest test )
		{
			foreach( ITestListener listener in InnerList )
			{
				listener.EndTest( test );
			}
		}

		#endregion
	}
}