using UnityEngine.InputSystem;

public class JumpCommand : ICommand
{
    private PlayerController player;
    private InputAction jumpAction;

    public JumpCommand(PlayerController player, InputActionReference jump)
    {
        this.player = player;
        jumpAction = jump.action;
    }

    public void Execute()
    {
        if (jumpAction.triggered)
            player.HandleJumpInput();
    }
}