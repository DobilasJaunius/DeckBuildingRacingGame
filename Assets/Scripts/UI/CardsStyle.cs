using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CardsStyle : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    [Header("Arch settings")]
    [SerializeField] float radius = .8f;
    [SerializeField] float spreadAngle = 60f;
    [SerializeField] float yOffset = .05f;
    [SerializeField] float headerScale = .1f;

    [Header("Input Settings")]
    [SerializeField] InputAction scrollCards;

    [Header("Card settings")]
    private int selectedIndex = 0;
    private bool firstSelection = true;

    private List<VisualElement> cards;
    private VisualElement currentSelected = null;
    private float currentSelectedAngle;

    void Start()
    {
        var container = uiDocument.rootVisualElement.Q("CardContainer");

        container.RegisterCallback<GeometryChangedEvent>(e => ArrangeCards(container));
        scrollCards.Enable();
    }

    private void Update() {

        if(scrollCards.WasPerformedThisFrame())
        {
            int direction = (int)scrollCards.ReadValue<float>();
            if (firstSelection)
            {
                selectedIndex -= direction;
                firstSelection = false;
            }

            if(direction == 1 && selectedIndex >= cards.Count-1) return;
            if(direction == -1 && selectedIndex <= 0) return;

            selectedIndex += direction;

            if(currentSelected != null)
            {
                OnCardHover(currentSelected, currentSelectedAngle, false);
            }

            float t = cards.Count == 1 ? 0 : (selectedIndex / (float)(cards.Count - 1)) * 2 - 1;
            float angleDeg = t * (spreadAngle / 2f);

            OnCardHover(cards[selectedIndex], angleDeg, true);

            currentSelected = cards[selectedIndex];
            currentSelectedAngle = angleDeg;
        }
    }

    void ArrangeCards(VisualElement container)
    {
        cards = container.Children().ToList();
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
