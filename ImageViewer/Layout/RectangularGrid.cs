namespace ClearCanvas.ImageViewer.Layout
{
    public class RectangularGrid
    {
        public int Rows;
        public int Columns;

        public class Location
        {
            public int Row;
            public int Column;

            public Location ParentGridLocation;

            public override bool Equals(object obj)
            {
                var other = obj as Location;
                if (other != null)
                    return other.Row == Row && other.Column == Column && Equals(other.ParentGridLocation, ParentGridLocation);
                return false;
            }

            public override int GetHashCode()
            {
                var hash = 0x7f4c2145 ^ Row.GetHashCode() ^ Column.GetHashCode();
                if (ParentGridLocation != null)
                    hash ^= ParentGridLocation.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as RectangularGrid;
            if (other != null)
                return other.Rows == Rows && other.Columns == Columns;
            return false;
        }

        public override int GetHashCode()
        {
            return 0x41c59841 ^ Rows.GetHashCode() ^ Columns.GetHashCode();
        }
    }
}