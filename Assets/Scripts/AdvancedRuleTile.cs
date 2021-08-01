using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class AdvancedRuleTile : RuleTile<AdvancedRuleTile.Neighbor>
{
    public TileBase[] connectingTiles;

    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Zero = 3;
        public const int One = 4;
        public const int Two = 5;
        public const int Three = 6;
        public const int Four = 7;
    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
        case Neighbor.This:
            return tile == this || tile == null;
        case Neighbor.NotThis:
            return tile != this && tile != null;
        case Neighbor.Zero:
        case Neighbor.One:
        case Neighbor.Two:
        case Neighbor.Three:
        case Neighbor.Four:
            return tile == connectingTiles[neighbor-Neighbor.Zero];
        }
        return base.RuleMatch(neighbor, tile);
    }
}