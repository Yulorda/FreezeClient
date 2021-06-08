using UnityEngine;

public class Unit : MonoBehaviour
{
    public enum State
    {
        Stay,
        Run
    }

    private State state;

    //View
    public Animation stayAnimation;
    public Animation runAnimation;

    public void MoveTo(Vector3 position, State state)
    {
        if (this.state != state)
        {
            switch (state)
            {
                case State.Stay:
                    Stay();
                    break;
                case State.Run:
                    Run();
                    break;
            }
        }

        transform.position = position;
    }

    [ContextMenu(nameof(Run))]
    private void Run()
    {
        stayAnimation.Stop();
        runAnimation.Play();
    }

    [ContextMenu(nameof(Stay))]
    private void Stay()
    {
        runAnimation.Stop();
        stayAnimation.Play();
    }
}
