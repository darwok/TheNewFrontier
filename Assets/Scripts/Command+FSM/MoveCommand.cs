using UnityEngine;
using UnityEngine.InputSystem;

public class MoveCommand : ICommand
{
    private PlayerController player;
    private InputAction moveAction;
    private InputAction sprintAction;

    public MoveCommand(PlayerController player, InputActionReference move, InputActionReference sprint)
    {
        this.player = player;
        moveAction = move.action;
        sprintAction = sprint.action;
    }

    public void Execute()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        bool sprinting = sprintAction.IsPressed();
        player.HandleMoveInput(input, sprinting);
    }
}