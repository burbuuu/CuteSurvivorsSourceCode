using System.Collections;
using TMPro;
using UnityEngine;

namespace FX
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private float lifetime = 0.6f;
        [SerializeField] private Vector2 floatOffset = new Vector2(0, 0.4f);

        private RectTransform rect;
        private Vector2 startPos;

        public void Init(int damage, Vector2 worldPosition)
        {
            rect = GetComponent<RectTransform>();

            // World-space positioning
            rect.position = worldPosition;

            text.text = damage.ToString();

            StartCoroutine(Animate());
        }

        private IEnumerator Animate()
        {
            float t = 0f;
            Vector3 start = rect.position;

            while (t < lifetime)
            {
                t += Time.deltaTime;
                float n = t / lifetime;

                rect.position = start + (Vector3)(floatOffset * n);
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}