#nullable enable
using HandSchool.Controls;
using HandSchool.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TappableCollectionView), typeof(CollectionViewRenderer2))]

namespace HandSchool.iOS.Renderers
{
    public class CollectionViewRenderer2 : CollectionViewRenderer
    {
        protected override GroupableItemsViewController<GroupableItemsView> CreateController(
            GroupableItemsView itemsView, ItemsViewLayout layout)
        {
            return new TappableCollectionViewController(itemsView, layout);
        }

        protected override ItemsViewLayout SelectLayout()
        {
            var itemSizingStrategy = ItemsView.ItemSizingStrategy;
            var itemsLayout = ItemsView.ItemsLayout;

            return itemsLayout switch
            {
                GridItemsLayout gridItemsLayout => new TappableGridViewLayout(gridItemsLayout, this, itemSizingStrategy),
                _ => base.SelectLayout()
            };
        }
    }
}