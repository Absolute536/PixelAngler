using System;
using Godot;
using SignalBusNS;

public partial class InGameTime : Node
{
    public static InGameTime Instance {get; private set;}

    private const int MinuteInDay = 1440;
    private const int MinuteInHour = 60;

    // so we "divide" 2pi by 1440 to get the value of each minute within a day
    // kinda like mapping the in game day (2pi) to real time minutes within a day
    // we'll get the in-game duration for 1 minute
    // so... 1440 is to 1 minute? (nah, this doesn't control this)
    private const float TimeCycleToRealMinute = Mathf.Tau / MinuteInDay;
    private const float GameSpeed = 1.0f;
    private int _initialHour = 12;

    private float _time; // time is in the in-game minutes passed, scaled to values on the sin function
    private int _previousMinute = -1; // this is used to determine if 1 minute (in game) has passed to raise time update events
    private TimeOfDay _previousTimeOfDay;
    public override void _Ready()
    {
        Instance = this;
        // IMPORTANT: the instantiation order of autoloads is dependent on the ordering int he Globals Tab
        // So autoloads that depend on another autoload must be instantiated later
        // So this must be below SignalBus

        // 10/12/2025: this one should be the fallback, should come from save file (if time == 0), then this, else use the save file
        _time = TimeCycleToRealMinute * MinuteInHour * _initialHour;
        _previousTimeOfDay = GetCurrentTimeOfDay();

    }

    public override void _PhysicsProcess(double delta)
    {
        // Increment time by delta, scaled to TimeCycleToRealMinute, multiplied by GameSpeed
        _time += (float) delta * TimeCycleToRealMinute * GameSpeed;
        // Then update the time (every frame?)
        UpdateInGameTime();
    }

    private void UpdateInGameTime()
    {
        int totalMinutesPassed = (int) (_time / TimeCycleToRealMinute); // truncate fractional, we only want an approximation of how many minutes passed
        // Since time is based on the total minutes (in game) elapsed, save state should save total minutes elapsed right?

        int day = totalMinutesPassed / MinuteInDay; // In game day
        int minutesPassedInDay = totalMinutesPassed % MinuteInDay; // Minutes passed within the day

        int hours = minutesPassedInDay / MinuteInHour; // In game hour
        int minutes = minutesPassedInDay % MinuteInHour; // In game minute

        if (_previousMinute != minutes)
        {
            _previousMinute = minutes; // update previous minute
            SignalBus.Instance.OnInGameTimeChanged(_time, day, hours, minutes);
        }

        if (_previousTimeOfDay != GetCurrentTimeOfDay())
        {
            _previousTimeOfDay = GetCurrentTimeOfDay();
            SignalBus.Instance.OnTimeOfDayChanged(this, EventArgs.Empty);
        }
    }

    public int GetHours()
    {
        // 0300 - 0659 -> Dawn
		// 0700 - 1659 -> Day
		// 1700 - 2059 -> Dusk
		// 2100 - 0259 -> Night

        // Hmm.. maybe consider using a property, cuz it's duplication
        int totalMinutesPassed = (int) (_time/TimeCycleToRealMinute);
        int minutesPassedInDay = totalMinutesPassed % MinuteInDay;

        return minutesPassedInDay / MinuteInHour;
    }

    public TimeOfDay GetCurrentTimeOfDay() // wait, maybe we'll just use this?
    {
        int hours = GetHours();
        if (hours >= 7 && hours <= 16)
			return TimeOfDay.Day;
		else if (hours >= 21 || hours <= 2)
			return TimeOfDay.Night;
		else if (hours >= 3 && hours <= 6)
			return TimeOfDay.Dawn;
		else
			return TimeOfDay.Dusk;
    }

}

public enum TimeOfDay
{
    Dawn,
    Day,
    Dusk, 
    Night
}