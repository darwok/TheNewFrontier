using UnityEngine.InputSystem;

public class DashCommand : ICommand
{
    private PlayerController player;
    private InputAction dashAction;

    public DashCommand(PlayerController player, InputActionReference dash)
    {
        this.player = player;
        dashAction = dash.action;
    }

    public void Execute()
    {
        if (dashAction.triggered)
            player.HandleDashInput();
    }
}