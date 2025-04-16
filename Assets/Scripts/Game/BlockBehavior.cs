using UnityEngine;

public class BlockBehavior : MonoBehaviour
{
    //Block material
    public Material BlockMaterial { get; private set; }
    public int MaterialIndex { get; private set; }

    //Value
    [SerializeField] Canvas _displayerCanva;
    TMPro.TextMeshProUGUI _valueDisplayer;

    Color _textColor;
    public bool IsVisible { get; private set; }

    public int Value { get; private set; }

    private void Awake()
    {
        _valueDisplayer = _displayerCanva.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        IsVisible = true;
    }

    private void Update()
    {
        //Make the text face the camera
        _displayerCanva.transform.LookAt(Camera.main.transform);
        _displayerCanva.transform.Rotate(0, 180, 0);
    }

    public void SetBlock(int value, Material mat, Color textColor,int matIndex)
    {
        BlockMaterial = mat;
        MaterialIndex = matIndex;

        gameObject.GetComponent<MeshRenderer>().material = BlockMaterial;
        _valueDisplayer.color = textColor;
        _textColor = textColor;

        _valueDisplayer.text = value.ToString();
        Value = value;
    }

    public void ChangeVisibility(bool isVisible)
    {
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();

        if (isVisible)
        {
            renderer.material.color = new Color(BlockMaterial.color.r, BlockMaterial.color.g, BlockMaterial.color.b, 1f);
            _valueDisplayer.color = _textColor;
            IsVisible = true;
        }
        else
        {
            renderer.material.color = new Color(BlockMaterial.color.r, BlockMaterial.color.g, BlockMaterial.color.b, 0.2f);
            _valueDisplayer.color = Color.clear;
            IsVisible = false;
        }
    }

    public void ChangeTextVisibility(bool isVisible)
    {
        _displayerCanva.enabled = isVisible;
    }
}
