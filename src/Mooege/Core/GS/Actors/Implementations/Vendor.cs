﻿/*
 * Copyright (C) 2011 mooege project
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System.Collections.Generic;
using Mooege.Common.MPQ.FileFormats.Types;
using Mooege.Core.GS.Common.Types.Math;
using Mooege.Core.GS.Map;
using Mooege.Core.GS.Players;
using Mooege.Net.GS.Message;
using Mooege.Net.GS.Message.Definitions.Trade;
using Mooege.Net.GS.Message.Definitions.World;
using Mooege.Core.GS.Common;
using Mooege.Core.Common.Items;
using Mooege.Core.Common.Items.ItemCreation;

namespace Mooege.Core.GS.Actors.Implementations
{
    [HandledSNO(178396 /* Fence_In_Town_01? */)] //TODO this is just a test, do it properly for all vendors?
    public class Vendor : InteractiveNPC
    {
        private InventoryGrid _vendorGrid;

        public Vendor(World world, int actorSNO, Vector3D position, Dictionary<int, TagMapEntry> tags)
            : base(world, actorSNO, position, tags)
        {
            this.Attributes[GameAttribute.MinimapActive] = true;
            _vendorGrid = new InventoryGrid(this, 1, 20, (int) EquipmentSlotId.Vendor);
            PopulateItems();
        }


        // TODO: Proper item loading from droplist?
        protected virtual List<Item> GetVendorItems()
        {
            var list = new List<Item>();

            /*var def = new ItemDefinition(189846, "Crafting_Tier_01A", null);
            list.Add(ItemGenerator.CreateItem(this, def));
            def = new ItemDefinition(189846, "Crafting_Tier_01B", null);
            list.Add(ItemGenerator.CreateItem(this, def));
            def = new ItemDefinition(189846, "Crafting_Tier_01C", null);
            list.Add(ItemGenerator.CreateItem(this, def));
            def = new ItemDefinition(189846, "Crafting_Tier_01D", null);
            list.Add(ItemGenerator.CreateItem(this, def)); */

            list.Add(ItemGenerator.GenerateRandom(this));
            list.Add(ItemGenerator.GenerateRandom(this));
            list.Add(ItemGenerator.GenerateRandom(this));
            list.Add(ItemGenerator.GenerateRandom(this));
            list.Add(ItemGenerator.GenerateRandom(this));
            list.Add(ItemGenerator.GenerateRandom(this));

            return list;
        }

        private void PopulateItems()
        {
            var items = GetVendorItems();
            if (items.Count > _vendorGrid.Columns)
            {
                _vendorGrid.ResizeGrid(1, items.Count);
            }

            foreach (var item in items)
            {
                item.Field3 = 1; // this is needed for inv items, should be handled in actor /fasbat
                _vendorGrid.AddItem(item);
            }

        }

        public override bool Reveal(Players.Player player)
        {
            if (!base.Reveal(player))
                return false;

            _vendorGrid.Reveal(player);
            return true;
        }

        public override bool Unreveal(Players.Player player)
        {
            if (!base.Reveal(player))
                return false;

            _vendorGrid.Unreveal(player);
            return true;
        }

        public override void OnTargeted(Player player, TargetMessage message)
        {
            player.InGameClient.SendMessage(new OpenTradeWindowMessage((int)this.DynamicID));
        }


        public virtual void OnRequestBuyItem(Players.Player player, Item item)
        {
            // TODO: Check gold here

            if (!player.Inventory.HasInventorySpace(item))
            {
                return;
            }

            // TODO: Remove the gold

            var newItem = new Item(this.World, item.SNOId, item.GBHandle.GBID, item.ItemType);
            var attributeCreators = new AttributeCreatorFactory().Create(item.ItemType);
            foreach (IItemAttributeCreator creator in attributeCreators)
            {
                creator.CreateAttributes(item);
            }
            player.Inventory.PickUp(newItem); // TODO: Dont use pickup? ;)
        }
    }
}
