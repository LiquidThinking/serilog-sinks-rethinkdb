// ReSharper disable NotAccessedVariable
// ReSharper disable RedundantAssignment

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace RethinkDBSample
{
	class Position
	{
		public double Lat { get; set; }
		public double Long { get; set; }
	}

	class Program
	{
		static void Main()
		{
			Log.Logger = new LoggerConfiguration()
				.Enrich.With<FixtureIdEnricher>()
				.WriteTo.ColoredConsole()
				.MinimumLevel.Debug()
				.WriteTo.Seq("http://localhost:5341")				
				.CreateLogger();

			Log.Verbose( "This app, {ExeName}, demonstrates the basics of using Serilog", "Demo.exe" );

			ProcessInput( new Position { Lat = 24.7, Long = 132.2 } );
			ProcessInput( new Position { Lat = 24.71, Long = 132.15 } );
			ProcessInput( new Position { Lat = 24.72, Long = 132.2 } );


			Log.Information( "Just biting {Fruit} number {Count}", "Apple", 12 );
			Log.ForContext<Program>().Information( "Just biting {Fruit} number {Count:0000}", "Apple", 12 );

			// ReSharper disable CoVariantArrayConversion
			Log.Information( "I've eaten {Dinner}", new[] { "potatoes", "peas" } );
			// ReSharper restore CoVariantArrayConversion

			Log.Information( "I sat at {@Chair}", new { Back = "straight", Legs = new[] { 1, 2, 3, 4 } } );
			Log.Information( "I sat at {Chair}", new { Back = "straight", Legs = new[] { 1, 2, 3, 4 } } );

			var fixtureState = new FixtureStateDummy
			{
				Team = new Team()
				{
					Id = 1,
					Name = "Test"
				},
				FixtureId = 1
			};
			Log.ForContext("FixtureState", fixtureState, true).Information( "Here is the FixtureState" );

			const int failureCount = 3;
			Log.Warning( "Exception coming up because of {FailureCount} failures...", failureCount );

			try
			{
				DoBad();
			}
			catch ( Exception ex )
			{
				Log.Error( ex, "There's those {FailureCount} failures", failureCount );
			}

			Log.Verbose( "This app, {ExeName}, demonstrates the basics of using Serilog", "Demo.exe" );

			try
			{
				DoBad();
			}
			catch ( Exception ex )
			{
				Log.Error( ex, "We did some bad work here." );
			}

			var result = 0;
			var divideBy = 0;
			try
			{
				result = 10 / divideBy;
			}
			catch ( Exception e )
			{
				Log.Error( e, "Unable to divide by {divideBy}", divideBy );
			}


			for ( var i = 0; i < 10; i++ )
			{
				Thread.Sleep( 500 );
				Log.Debug( "Count: {Counter}", i );
			}

			Log.Fatal( "That's all folks" );
			Console.ReadKey( true );
		}

		static void DoBad()
		{
			throw new InvalidOperationException( "Everything's broken!" );
		}

		static readonly Random Rng = new Random();

		static void ProcessInput( Position sensorInput )
		{
			var sw = new Stopwatch();
			sw.Start();
			Log.Debug( "Processing some input on {MachineName}...", Environment.MachineName );
			Thread.Sleep( Rng.Next( 0, 100 ) );
			sw.Stop();

			Log.Information( "Processed {@SensorInput} in {Time:000} ms", sensorInput, sw.ElapsedMilliseconds );

		}
	}

	internal class FixtureIdEnricher: ILogEventEnricher {
		public void Enrich( LogEvent logEvent, ILogEventPropertyFactory propertyFactory )
		{
			logEvent.AddPropertyIfAbsent( new LogEventProperty( "Context", new StructureValue( new List<LogEventProperty>
			                                                                                   {
				                                                                                   new LogEventProperty( "FixtureId", new ScalarValue( 1 ) ),
																								   new LogEventProperty( "Site", new ScalarValue( "lastmanstands")),
																								   new LogEventProperty( "Action", new ScalarValue( "publishing"  )  )

			                                                                                   } ) ) );
		}
	}

	public class FixtureStateDummy
	{
		public int FixtureId { get; set; }
		public Team Team { get; set; }

	}

	public class Team
	{
		public int Id { get; set; }
		public string Name { get; set; }

	}
}
