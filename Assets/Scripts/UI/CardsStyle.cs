using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CardsStyle : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    [Header("Arch settings")]
    [SerializeField] float radius = .8f;
    [SerializeField] float spreadAngle = 60f;
    [SerializeField] float yOffset = .05f;
    [SerializeField] float headerScale = .1f;

    void Start()
    {
        var container = uiDocument.rootVisualElement.Q("CardContainer");

        container.RegisterCallback<GeometryChangedEvent>(e => ArrangeCards(container));
    }

    void ArrangeCards(VisualElement container)
    {
        var cards = container.Children().ToList();
        int count = cards.Count;

        float panelHeight = container.resolvedStyle.height;
        float panelWidth = container.resolvedStyle.width;

        // Make radius and offset relative to panel size
        float actualRadius = panelHeight * radius;         // radius is now 0..2 range, e.g. 0.8
        float actualOffset = panelHeight * yOffset; // same, e.g. 0.3

        for (int i = 0; i < count; i++)
        {
            float t = count == 1 ? 0 : (i / (float)(count - 1)) * 2 - 1;
            float angleDeg = t * (spreadAngle / 2f);
            float angleRad = angleDeg * Mathf.Deg2Rad;

            float x = Mathf.Sin(angleRad) * actualRadius;
            float y = -Mathf.Cos(angleRad) * actualRadius + actualRadius;

            var card = cards[i];
            card.style.position = Position.Absolute;
            card.style.left = x + panelWidth / 2 - card.resolvedStyle.width / 2;
            card.style.top = y + actualOffset;

            card.style.rotate = new Rotate(angleDeg);
            card.style.transformOrigin = new TransformOrigin(Length.Percent(50), Length.Percent(50));

            //Subscribe to on hover and on leave events
            card.RegisterCallback<MouseEnterEvent>(e => OnCardHover(card, angleDeg, true));
            card.RegisterCallback<MouseLeaveEvent>(e => OnCardHover(card, angleDeg, false));

            //Change header size to 10% of the card
            card.RegisterCallback<GeometryChangedEvent>(e =>
            {
                var label = card.Q<Label>("CardLabel");
                label.style.fontSize = card.resolvedStyle.height * headerScale; // 10% of card height
            });
            
        }
    }

    void OnCardHover(VisualElement card, float angleDeg, bool hovering)
    {
        float offset = hovering ? -20f : 0f; // how far to move along local Y

        float angleRad = angleDeg * Mathf.Deg2Rad;

        // Local Y axis direction, rotated by the card's angle
        float dx = -Mathf.Sin(angleRad) * offset;
        float dy = Mathf.Cos(angleRad) * offset;

        card.style.translate = new Translate(dx, dy);
    }

}
