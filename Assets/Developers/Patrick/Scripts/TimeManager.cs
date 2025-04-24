using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    // Dictionary of all time scale changes and how long they last for
    private Dictionary<float, float> _timeList = new Dictionary<float, float>();

    // Values
    private float _defaultTimescale = 1.0f;
    private float _currentTimescale = 1.0f;
    private float _checkTimer = 0.0f;
    private bool _isNotDefaultTimescale = false;

    private void FixedUpdate()
    {
        // Doesn't work for default timescales below 1.0f

        // Check other timescales if they exist
        if (_isNotDefaultTimescale)
        {
            // Decrease the time of each value
            foreach (float key in _timeList.Keys.ToArray())
            {
                if (_timeList[key] <= 0.0f) // If the time has ended remove from dictionary
                {
                    _timeList.Remove(key);
                    continue;
                }

                _timeList[key] -= Time.unscaledDeltaTime;
            }

            // When dictionary is empty just use default time scale
            if(_timeList.Count <= 0)
            {
                _isNotDefaultTimescale = false;
                _currentTimescale = _defaultTimescale;
                Time.timeScale = _defaultTimescale;
                return;
            }

            _checkTimer -= Time.unscaledDeltaTime;

            // Checks for lowest timescale when previous one has finished
            if (_checkTimer <= 0.0f)
            {
                _currentTimescale = GetLowestTimescale();
                Time.timeScale = _currentTimescale;
                if(_currentTimescale != _defaultTimescale)
                {
                    _checkTimer = _timeList[_currentTimescale];
                }
            }
        }
    }

    // Changes the default timescale
    public void SetDefaultTimescale(float timescale)
    {
        _defaultTimescale = timescale;

        _currentTimescale = GetLowestTimescale();
    }

    public float GetDefaultTimescale()
    {
        return _defaultTimescale;
    }

    // Adds a timescale to the dictionary for the specified duration
    public void AddTimescaleForDuration(float timescale, float duration)
    {
        _isNotDefaultTimescale = true;

        // If the timescale doesn't already exist, add it to the dictionary
        if (!_timeList.ContainsKey(timescale))
        {
            _timeList.Add(timescale, duration);

            // Check if it is now the lowest timescale
            if(timescale < _currentTimescale)
            {
                _currentTimescale = timescale;
                Time.timeScale = _currentTimescale;
                _checkTimer = duration;
            }

            return;
        }

        // If it already exists change the duration to the higher of the two
        if (_timeList[timescale] < duration)
        {
            _timeList[timescale] = duration;
        }
    }

    // Checks the timescales for the lowest value and returns it
    private float GetLowestTimescale()
    {
        float lowestTimescale = _defaultTimescale;
        foreach(float key in _timeList.Keys.ToArray())
        {
            if (lowestTimescale > key)
            {
                lowestTimescale = key;
            }
        }

        return lowestTimescale;
    }
}
