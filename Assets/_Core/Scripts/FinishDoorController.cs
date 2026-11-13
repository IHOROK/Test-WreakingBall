using UnityEngine;

public class FinishDoorController : MonoBehaviour
{
    [SerializeField] private float openDistance = 5f;

    private Transform _player;
    private Transform _goal;
    private GameObject _leftPanel;
    private GameObject _rightPanel;
    private Material _doorMaterial;
    private Vector3 _leftClosedPosition;
    private Vector3 _rightClosedPosition;

    public bool IsOpen { get; private set; }

    public void Construct(Transform player, Transform goal, Material doorMaterial)
    {
        _player = player;
        _goal = goal;
        _doorMaterial = doorMaterial;
        Vector3 direction = (goal.position - player.position).normalized;
        Vector3 center = goal.position - direction * 0.4f;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

        _leftPanel = CreatePanel("Door Left", center - rotation * Vector3.right * 0.27f, rotation);
        _rightPanel = CreatePanel("Door Right", center + rotation * Vector3.right * 0.27f, rotation);
        _leftClosedPosition = _leftPanel.transform.position;
        _rightClosedPosition = _rightPanel.transform.position;
    }

    public void UpdateForPlayer()
    {
        if (IsOpen || _player == null || _goal == null)
            return;

        if (Vector3.Distance(_player.position, _goal.position) <= openDistance)
            Open();
    }

    private void Open()
    {
        IsOpen = true;
        _leftPanel.transform.position = _leftClosedPosition - _leftPanel.transform.right * 0.65f;
        _rightPanel.transform.position = _rightClosedPosition + _rightPanel.transform.right * 0.65f;
    }

    private GameObject CreatePanel(string panelName, Vector3 position, Quaternion rotation)
    {
        GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        panel.name = panelName;
        panel.transform.SetPositionAndRotation(position, rotation);
        panel.transform.localScale = new Vector3(0.54f, 1.5f, 0.14f);

        Renderer renderer = panel.GetComponent<Renderer>();
        if (renderer != null && _doorMaterial != null)
            renderer.sharedMaterial = _doorMaterial;

        return panel;
    }
}
