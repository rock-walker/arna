namespace AP.Server
{
	public static class Topics
	{
		public static class Commands
		{
			/// <summary>
			/// conference/commands
			/// </summary>
			public const string Path = "commands";

			public static class Subscriptions
			{
				/// <summary>
				/// sessionless
				/// </summary>
				public const string Sessionless = "sessionless";
				/// <summary>
				/// seatsavailability
				/// </summary>
				public const string Anchorsavailability = "anchorsavailability";
				/// <summary>
				/// log
				/// </summary>
				public const string Log = "log";
			}
		}

		public static class Events
		{
			/// <summary>
			/// conference/events
			/// </summary>
			public const string Path = "events";

			public static class Subscriptions
			{
				/// <summary>
				/// log
				/// </summary>
				public const string Log = "log";
				/// <summary>
				/// Registration.RegistrationPMNextSteps
				/// </summary>
				public const string RegistrationPMNextSteps = "AP.Registration.RegistrationPMNextSteps";
				/// <summary>
				/// Registration.PricedOrderViewModelGeneratorV3
				/// </summary>
				public const string PricedOrderViewModelGenerator = "AP.Registration.PricedOrderViewModelGenerator";
				/// <summary>
				/// Registration.ConferenceViewModelGenerator
				/// </summary>
				public const string WorkshopViewModelGenerator = "AP.Registration.WorkshopViewModelGenerator";
			}
		}

		public static class EventsOrders
		{
			/// <summary>
			/// conference/eventsOrders
			/// </summary>
			public const string Path = "eventsOrders";

			public static class Subscriptions
			{
				/// <summary>
				/// logOrders
				/// </summary>
				public const string LogOrders = "logOrders";
				/// <summary>
				/// Registration.RegistrationPMOrderPlacedOrders
				/// </summary>
				public const string RegistrationPMOrderPlacedOrders = "AP.Registration.RegistrationPMOrderPlacedOrders";
				/// <summary>
				/// Registration.RegistrationPMNextStepsOrders
				/// </summary>
				public const string RegistrationPMNextStepsOrders = "AP.Registration.RegistrationPMNextStepsOrders";
				/// <summary>
				/// Registration.OrderViewModelGeneratorOrders
				/// </summary>
				public const string OrderViewModelGeneratorOrders = "AP.Registration.OrderViewModelGeneratorOrders";
				/// <summary>
				/// Registration.PricedOrderViewModelOrders
				/// </summary>
				public const string PricedOrderViewModelOrders = "AP.Registration.PricedOrderViewModelOrders";
				/// <summary>
				/// Registration.SeatAssignmentsViewModelOrders
				/// </summary>
				public const string AnchorAssignmentsViewModelOrders = "AP.Registration.AnchorAssignmentsViewModelOrders";
				/// <summary>
				/// Registration.SeatAssignmentsHandlerOrders
				/// </summary>
				public const string AnchorAssignmentsHandlerOrders = "AP.Registration.AnchorAssignmentsHandlerOrders";
				/// <summary>
				/// Workshop.OrderEventHandlerOrders
				/// </summary>
				public const string OrderEventHandlerOrders = "AP.Workshop.OrderEventHandlerOrders";
			}
		}

		public static class EventsAvailability
		{
			/// <summary>
			/// conference/eventsAvailability
			/// </summary>
			public const string Path = "eventsAvailability";

			public static class Subscriptions
			{
				/// <summary>
				/// logAvail
				/// </summary>
				public const string LogAvail = "logAvail";
				/// <summary>
				/// Registration.RegistrationPMNextStepsAvail
				/// </summary>
				public const string RegistrationPMNextStepsAvail = "AP.Registration.RegistrationPMNextStepsAvail";
				/// <summary>
				/// Registration.ConferenceViewModelAvail
				/// </summary>
				public const string WorkshopViewModelAvail = "AP.Registration.WorkshopViewModelAvail";
			}
		}

	}
}
