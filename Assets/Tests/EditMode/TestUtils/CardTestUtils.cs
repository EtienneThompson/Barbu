namespace Barbu.Tests.EditMode.TestUtils
{
    using System.Reflection;
    using Barbu.Core;
    using Barbu.Core.Events;
    using Barbu.Gameplay;
    using UnityEngine;

    /// <summary>
    /// Card is a MonoBehaviour, so tests build one directly on a throwaway
    /// GameObject via InitializeData rather than going through ICardFactory /
    /// Zenject, which need scene and resource setup we don't want in unit tests.
    /// Card's own stateMachine/eventsController are normally set by Zenject's
    /// [Inject] Init(), which never runs here, so we call it ourselves with fakes -
    /// pass in the test's own FakeStateMachine when assertions need to observe
    /// what PlayCard() does to it (e.g. NumCardsPlayed).
    /// Callers are responsible for destroying the returned Card's GameObject
    /// (e.g. via Object.DestroyImmediate in [TearDown]).
    /// </summary>
    public static class CardTestUtils
    {
        private static readonly FieldInfo MeshRendererField =
            typeof(Card).GetField("meshRenderer", BindingFlags.NonPublic | BindingFlags.Instance);

        public static Card CreateCard(
            string suit,
            string rank,
            string playerId = "1",
            IStateMachine stateMachine = null,
            IEventsController eventsController = null)
        {
            var gameObject = new GameObject($"TestCard_{suit}{rank}");
            var card = gameObject.AddComponent<Card>();
            card.Init(stateMachine ?? new FakeStateMachine(), eventsController ?? new FakeEventsController());
            card.InitializeData(suit, rank, playerId);
            return card;
        }

        /// <summary>
        /// Same as CreateCard, but also attaches a MeshRenderer with a built-in-shader
        /// material so Card.Highlight()/RemoveHighlight() - which dereference the
        /// renderer's material - can run without going through Card.InitializeGameObject's
        /// real Resources.Load(card art)/Shader.Find(custom shader) pipeline.
        /// </summary>
        public static Card CreateCardWithRenderer(
            string suit,
            string rank,
            string playerId = "1",
            IStateMachine stateMachine = null,
            IEventsController eventsController = null)
        {
            var card = CreateCard(suit, rank, playerId, stateMachine, eventsController);
            var meshRenderer = card.gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Standard"));
            MeshRendererField.SetValue(card, meshRenderer);
            return card;
        }
    }
}
