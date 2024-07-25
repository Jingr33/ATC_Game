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
/// Characteristic of flight according to _time schedule
/// </summary>
public enum FlightStatus
{
    InTime,
    Delayed,
    Diverted
}

public enum WeightCat
{
    A, 
    B, 
    C, 
    D
}

/// <summary>
/// enumeration of all maps in the game
/// </summary>
public enum Maps
{
    Prague
}

/// <summary>
/// Possibilities of autopilot control.
/// </summary>
public enum AutopilotOperation
{
    Unknown,
    TakeOff,
    Landing,
    LeftTurn,
    RightTurn,
}