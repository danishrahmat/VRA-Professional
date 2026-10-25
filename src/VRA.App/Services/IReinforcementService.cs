namespace VRA.App.Services;

public interface IReinforcementService
{
    void ShowAnimation(string display);
    void PlayRandomVideo(string display, string folderId);
    void PlaySound(string display);
    void StopAll();
}