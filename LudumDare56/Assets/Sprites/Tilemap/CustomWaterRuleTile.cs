using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "2D/Tiles/TypeRuleTile")]
public class NewCustomRuleTile : RuleTile<NewCustomRuleTile.Neighbor> {
    public TileBase Bridge;
    public TileBase Water;

    public class Neighbor : RuleTile.TilingRule.Neighbor {
		public const int Bridge = 3;
		public const int Water = 4;
		public const int BridgeOrWater = 5;
		public const int NotBridgeOrWater = 6;
	}

    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor)
		{
			case Neighbor.Bridge: return tile == Bridge;
			case Neighbor.Water: return tile == Water;
			case Neighbor.BridgeOrWater: return tile == Water || tile == Bridge;
			case Neighbor.NotBridgeOrWater: return !(tile == Water || tile == Bridge);
		}
        return base.RuleMatch(neighbor, tile);
    }
}