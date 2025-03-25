using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private float _defaultTimescale = 1.0f;

    private Dictionary<float, float> _timeList = new Dictionary<float, float>();

    private float _currentTimescale = 1.0f;
    private float _checkTimer = 0.0f;
    private bool _isNotDefaultTimescale = false;

    private void Update()
    {
        if (_isNotDefaultTimescale)
        {
            foreach (float key in _timeList.Keys.ToArray())
            {
                if (_timeList[key] <= 0.0f)
                {
                    _timeList.Remove(key);
                    continue;
                }

                _timeList[key] -= GetRealtimeDeltaTime();
            }

            if(_timeList.Count <= 0)
            {
                _isNotDefaultTimescale = false;
                _currentTimescale = _defaultTimescale;
                Time.timeScale = _defaultTimescale;
                return;
            }

            _checkTimer -= GetRealtimeDeltaTime();

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

    public float GetRealtimeDeltaTime()
    {
        return Time.deltaTime / _currentTimescale;
    }

    public float GetRealtimeFixedDeltaTime()
    {
        return Time.fixedDeltaTime / _currentTimescale;
    }

    public void SetDefaultTimescale(float timescale)
    {
        _defaultTimescale = timescale;

        if (_defaultTimescale < _currentTimescale)
        {
            _currentTimescale = _defaultTimescale;
        }
    }

    public void AddTimescaleForDuration(float timescale, float duration)
    {
        _isNotDefaultTimescale = true;
        if (!_timeList.ContainsKey(timescale))
        {
            _timeList.Add(timescale, duration);

            if(timescale < _currentTimescale)
            {
                _currentTimescale = timescale;
                Time.timeScale = _currentTimescale;
                _checkTimer = duration;
            }

            return;
        }

        if (_timeList[timescale] < duration)
        {
            _timeList[timescale] = duration;
        }
    }

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
