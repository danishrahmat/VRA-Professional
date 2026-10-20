namespace VRA.App.Services;

public interface IReinforcementService
{
    void ShowAnimation(string display);
    void PlaySound(string display);
    void StopAll();
}