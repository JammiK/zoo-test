namespace ZW.Logic.Models.Movements
{
    public readonly struct AnimalMotionCommand
    {
        public readonly int Uid;
        public readonly AnimalMotion Motion;

        public AnimalMotionCommand(int uid, AnimalMotion motion)
        {
            Uid = uid;
            Motion = motion;
        }
    }
}
