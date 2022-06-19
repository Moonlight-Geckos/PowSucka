using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumFillBar : ProgressBar
{
    private void Start()
    {
        EventsPool.PickedupObjectEvent.AddListener(AddToVacum);
        EventsPool.ChangePhaseEvent.AddListener(SliderChange);
    }
    private void AddToVacum(FillType y)
    {
        if (y == FillType.Diamond)
            return;
        byte val = StaticValues.GetFillPercent(y);
        UpdateValue(slider.value + val / 100f);
    }
    private void SliderChange(bool weaponMode)
    {
        if (!weaponMode)
            UpdateValue(0);
        else
        {
            IEnumerator slideDown()
            {
                yield return null;
                float duration = Observer.weaponDuration;
                float elapsed = 0;
                Color color = colorGradient.Evaluate(1);
                while (elapsed < duration)
                {
                    UpdateValue(slider.value - Time.deltaTime/duration, color);
                    yield return new WaitForEndOfFrame();
                    elapsed += Time.deltaTime;
                }
            }
            StartCoroutine(slideDown());
        }
    }
}
