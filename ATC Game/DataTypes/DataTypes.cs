/// <summary>
/// Describe if a flight is arrival or departure.
/// </summary>
public enum OperationType
{
    Arrival,
    Departure
}

/// <summary>
/// Characteristic status for actual part of flight
/// </summary>
public enum FlightSection
{
    Entry,
    Approach,
    Final,
    Landing,
    MissedApproach,
    GoAround,
    TakeOff,
    Departure,
}

/// <summary>
/// Characteristic of flight according to time schedule
/// </summary>
public enum FlightStatus
{
    InTime,
    Delayed,
    Diverted
}