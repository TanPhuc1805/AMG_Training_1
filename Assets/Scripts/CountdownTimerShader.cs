using UnityEngine;
using TMPro;
using System;

public class CountdownTimerShader : MonoBehaviour
{
    [System.Serializable]
    public class CountdownUnit
    {
        public Renderer quadRenderer;   // Quad's Renderer (Shader Material)
        public TextMeshPro numberText;  // TextMeshPro for the countdown number
        public TextMeshPro labelText;   // TextMeshPro for the unit (D/H/M/S)
        public float maxValue;          // Maximum value (e.g., 24 for hours)
        private Material material;      // Shader material reference

        public void Initialize()
        {
            if (quadRenderer != null)
            {
                material = quadRenderer.material; // Get a material instance
            }
        }

        public void UpdateUnit(int value)
        {
            if (material != null)
            {
                float fillValue = (float)value / maxValue;
                material.SetFloat("_Fill_Progress", fillValue); // Update shader property
            }
            if (numberText != null)
            {
                numberText.text = value.ToString("00"); // Format as 2-digit numbers
            }
        }
    }

    public CountdownUnit days;
    public CountdownUnit hours;
    public CountdownUnit minutes;
    public CountdownUnit seconds;

    public DateTime targetTime; // The end time for the countdown

    void Start()
    {
        // Initialize shader materials
        days.Initialize();
        hours.Initialize();
        minutes.Initialize();
        seconds.Initialize();

        // Set a target time (default: 3 day from now)
        targetTime = DateTime.UtcNow.AddDays(3);
    }

    void Update()
    {
        UpdateCountdown();
    }

    void UpdateCountdown()
    {
        TimeSpan remainingTime = targetTime - DateTime.UtcNow;
        if (remainingTime.TotalSeconds <= 0)
        {
            SetToZero();
            return;
        }

        days.UpdateUnit(remainingTime.Days);
        hours.UpdateUnit(remainingTime.Hours);
        minutes.UpdateUnit(remainingTime.Minutes);
        seconds.UpdateUnit(remainingTime.Seconds);
    }

    void SetToZero()
    {
        days.UpdateUnit(0);
        hours.UpdateUnit(0);
        minutes.UpdateUnit(0);
        seconds.UpdateUnit(0);
    }
}
