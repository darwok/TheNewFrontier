using UnityEngine.InputSystem;

public class AttackCommand : ICommand
{
    private PlayerController player;
    private InputAction attackAction;

    public AttackCommand(PlayerController player, InputActionReference attack)
    {
        this.player = player;
        attackAction = attack.action;
    }

    public void Execute()
    {
        if (attackAction.triggered)
            player.HandleAttackInput();
    }
}