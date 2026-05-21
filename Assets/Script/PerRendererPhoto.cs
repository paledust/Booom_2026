using UnityEditor;
using UnityEngine;

public class PerRendererPhoto : PerRendererBehavior
{
    private SpriteRenderer spriteRenderer;
    private const string PHOTO_X_OFFSET = "_OffsetX";
    private const string PHOTO_Y_OFFSET = "_OffsetY";
    private const string PHOTO_SCALE_X = "_ScaleX";
    private const string PHOTO_SCALE_Y = "_ScaleY";
    void Awake()
    {
        spriteRenderer = meshRenderer as SpriteRenderer;
        if(spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }
    protected override void InitProperties()
    {
        base.InitProperties();
        mpb.SetFloat(PHOTO_X_OFFSET, transform.position.x);
        mpb.SetFloat(PHOTO_Y_OFFSET, transform.position.y);

        Vector2 size = 5 * transform.localScale;
        float scale = Mathf.Max(size.x, size.y) / 14;
        mpb.SetFloat(PHOTO_SCALE_X, scale);
        mpb.SetFloat(PHOTO_SCALE_Y, scale);
    }
#if UNITY_EDITOR
    protected override void UpdateProperties()
    {
        if(!EditorApplication.isPlaying)
        {
            mpb.SetFloat(PHOTO_X_OFFSET, transform.position.x);
            mpb.SetFloat(PHOTO_Y_OFFSET, transform.position.y);
            
            Vector2 size = 5 * transform.localScale;
            float scale = Mathf.Max(size.x, size.y) / 14;
            mpb.SetFloat(PHOTO_SCALE_X, scale);
            mpb.SetFloat(PHOTO_SCALE_Y, scale);
        }
    }
#endif
}
