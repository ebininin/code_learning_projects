namespace ObjectMethods
{
    public class Student
    {
        public string Name, Major;
        public double Gpa;

        public Student(string name, string major, double gpa)
        {
            Name = name;
            Major = major;
            Gpa = gpa;
        }

        public bool IsHonored()
        {
            if (Gpa >= 3.5)
            {
                return true;
            }
            return false;
        }
    }
}