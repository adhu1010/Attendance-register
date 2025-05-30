using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;


abstract class Person
{
    public string Name { get; set; }
    public String Id { get; set; }

    public Person(string name, String id)
    {
        Name = name;
        Id = id;
    }

    public abstract void MarkAttendance(DateTime Date, Dictionary<string,bool> periodAttendance);
    public abstract Dictionary<string, double> GetAttendancePercentage();
    public abstract void DisplayInfo(Dictionary<String, Double> allPeriodAttendance);
}

class Student : Person
{
    private Dictionary<DateTime, Dictionary<string, bool>> AttendanceRecord;
    private Dictionary<string, List<string>> Subjects { get; set; } = new Dictionary<string, List<string>>()
    {
        { "Monday", new List<string> { "DS", "DBMS", "JAVA", "AI", "ML", "PE" } },
        { "Tuesday", new List<string> { "AI", "DS", "PE", "ML", "DBMS", "JAVA" } },
        { "Wednesday", new List<string> { "ML", "AI", "DS", "PE", "JAVA", "DBMS" } },
        { "Thursday", new List<string> { "DS", "ML", "JAVA", "DBMS", "AI", "PE" } },
        { "Friday", new List<string> { "JAVA", "DS", "AI", "ML", "PE", "DBMS" } }
    };
    
    private const int TotalSubjects = 6;
    private const int TotalPeriods = 6;

    public Student(string name, String id) : base(name, id)
    {
        AttendanceRecord = new Dictionary<DateTime, Dictionary<string, bool>>();

    }
    /*
    public void AddSubject(string subjectName, List<string> periods)
    {
        if (Subjects.Count >= TotalSubjects)
        {
            throw new InvalidOperationException("Cannot add more subjects, limit reached.");
        }
        if (periods == null || periods.Count != TotalPeriods)
        {
            throw new ArgumentException("Periods must be a list of exactly 6 periods.");
        }
        Subjects[subjectName] = periods;
    }
    */

    public override void MarkAttendance(DateTime Date, Dictionary<string, bool> periodattendance)
    {
        if (periodattendance == null)
        {
            throw new ArgumentNullException(nameof(periodattendance));
        }

        foreach (KeyValuePair<string, bool> period in periodattendance)
        {
            if (!AttendanceRecord.ContainsKey(Date))
            {
                AttendanceRecord[Date] = new Dictionary<string, bool>();
            }
            AttendanceRecord[Date][period.Key] = period.Value;
        }
    }

    public override Dictionary<string, double> GetAttendancePercentage() 
    {
        if (AttendanceRecord.Count == 0)
        {
            return new Dictionary<string, double>(); 
        }

        List<string> allPeriods = Subjects.Values.SelectMany(p => p).Distinct().ToList();
        Dictionary<string, double> allPeriodAttendance = new Dictionary<string, double>();

        foreach (var period in allPeriods)
        {
            var totalPresent = 0;
            var totalPeriods = 0;

            foreach (var dateRecord in AttendanceRecord.Values)
            {
                foreach (var attendance in dateRecord)
                {
                    if (attendance.Key == period)
                    {
                        totalPeriods++;
                        if (attendance.Value)
                        {
                            totalPresent++;
                        }
                    }
                }
            }

            allPeriodAttendance[period] = totalPeriods > 0 ? (double)totalPresent / totalPeriods * 100 : 0.0;
        }

        return allPeriodAttendance; 
    }

    public override void DisplayInfo(Dictionary<String,Double> allPeriodAttendance)
    {
        Console.WriteLine("Attendance Record");
        Console.WriteLine($"Student Name: {Name}, ID: {Id}");
        Console.WriteLine("Subjects and Attendance Percentage");
        List<string> allPeriods = Subjects.Values.SelectMany(p => p).Distinct().ToList();
        foreach (var subject in allPeriods)
        {
            Console.WriteLine($"{subject} :{allPeriodAttendance[subject]}%");

        }

    }

    public static void Main(string[] args)
    {
        Console.WriteLine("Enter Student Details");
        Console.Write("Name: ");
        string name = Console.ReadLine();
        Console.Write("ID: ");
        String id =Console.ReadLine();
        Student student = new Student(name, id);
        List<string> days= new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };

        /*
        Console.WriteLine("Enter Time Table  from Monday To Friday:");
        List<string> days = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };    
        foreach (string day in days)
        {
            Console.WriteLine($"Enter periods for {day} (comma separated): ");
            string input = Console.ReadLine();
            List<string> periods = input.Split(',').Select(p => p.Trim()).ToList();
            if (periods.Count != 6)
            {
                Console.WriteLine("Please enter exactly 6 periods.");
                return;
            }
            student.AddSubject(day, periods);
        }
        */

        Dictionary<string, bool> periodAttendance = new Dictionary<string, bool>();
        while (true)
            {
                Console.WriteLine("Enter a date (yyyy-MM-dd): ");
                string inputDate = Console.ReadLine();


                if (DateTime.TryParse(inputDate, out DateTime parsedDate))
                {
                    string dayOfWeek = parsedDate.ToString("dddd");

                //Console.WriteLine(parsedDate.ToString("dddd"));
                    if (!days.Contains(dayOfWeek))
                    {
                        Console.WriteLine("Date is not a valid school day. Please enter a valid date.");
                        continue;
                    }
                  Console.WriteLine("Enter the Attendance");
                    foreach (var subject in student.Subjects)
                    {
                        if (subject.Key==dayOfWeek)
                        {
                            foreach (var period in subject.Value)
                            {
                                Console.Write($"Is {name} present for {period} ? (yes/no): ");
                                string response = Console.ReadLine().Trim().ToLower();
                                if (response == "yes")
                                {
                                    periodAttendance[period] = true;
                                }
                                else if (response == "no")
                                {
                                    periodAttendance[period] = false;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input. Please enter 'yes' or 'no'.");
                                    continue;
                                }
                            }
                        }
                    }


                    
                }
                else
                {
                    Console.WriteLine("Invalid date format. Please try again.");
                }
            
            student.MarkAttendance(parsedDate, periodAttendance);
            Console.WriteLine("Attendance marked successfully.");
            Console.WriteLine("Do you want to mark attendance for another date? (yes/no)");
            string continueResponse = Console.ReadLine().Trim().ToLower();
            if (continueResponse != "yes")
            {
                break;
            }
        }
        student.DisplayInfo(student.GetAttendancePercentage());


    }
} 