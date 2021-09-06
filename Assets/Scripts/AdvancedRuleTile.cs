using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[CreateAssetMenu]
public class AdvancedRuleTile : RuleTile<AdvancedRuleTile.Neighbor>
{
    public TileBase[] connectingTiles;
    public TileBase[] any;
    public TileBase[] alsoThis;

    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Zero = 3;
        public const int One = 4;
        public const int Two = 5;
        public const int Three = 6;
        public const int Four = 7;
        public const int Any = 8;
        public const int NotNull = 9;
    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
        case Neighbor.This:
            return tile == this || alsoThis.Contains(tile);
        case Neighbor.NotThis:
            return tile != this && tile != null;
        case Neighbor.Zero:
        case Neighbor.One:
        case Neighbor.Two:
        case Neighbor.Three:
        case Neighbor.Four:
            return tile == connectingTiles[neighbor-Neighbor.Zero];
        case Neighbor.Any:
            return any.Contains(tile);
        case Neighbor.NotNull:
            return tile != null;
        }
        return base.RuleMatch(neighbor, tile);
    }
}