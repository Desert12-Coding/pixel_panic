using UnityEngine;

public class freezeplayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        [header("Freeze settings")]
        public int requiredPresses = 6;
        public float freezeInterval = 10f;

        [header("color changer")]
        public Color frozencolor = new color(0.6f, 0.9f, 1f); 
        
        [Header("UI")]
        public UnityEngine.UI.Text pressCounterText;

        private int currentPressCount = 0;
        Private int Rigidbody2D;
        Private SpriteRenderer spriteRenderer;
        Private bool isFrozen = false;
        Private float Timer = 0f;
        private playerInput PlayerInput;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
