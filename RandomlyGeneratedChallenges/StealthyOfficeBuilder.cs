using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RandomlyGeneratedChallenges
{
    public class StealthyOfficeBuilder : OfficeBuilderStandard
    {
        public override void Setup(LevelBuilder lg, RoomController room, System.Random rng)
        {
            this.gameObject.SetActive(true);
            base.Setup(lg, room, rng);
        }

        private void RemoveDoor(IntVector2 pos, Direction dir, Door door)
        {
            TileController tc = room.ec.TileFromPos(pos);
            tc.doorDirsSpace.Remove(dir);
            tc.doorDirs.Remove(dir);
            tc.doors.Remove(door);
            if (tc.doors.Count == 0)
            {
                tc.doorHere = false;
            }
            if (tc.room == this.room) return;
            tc.room.doorDirs.Remove(dir);
            tc.room.doors.Remove(door);
        }

        public override void Finish()
        {
            base.Finish();
            foreach (Door door in this.room.doors)
            {
                if (door is Window) continue; //dont remove windows, those are fine.
                RemoveDoor(door.position, door.direction, door);
                room.ec.UpdateTile(door.position);
                IntVector2 secondPos = door.position + door.direction.ToIntVector2();
                RemoveDoor(secondPos, door.direction.GetOpposite(), door);
                room.ec.UpdateTile(secondPos);
                if (door != null)
                {
                    GameObject.Destroy(door.gameObject);
                }
            }
            room.doorDirs.Clear();
            room.doors.Clear();
        }
    }
}
