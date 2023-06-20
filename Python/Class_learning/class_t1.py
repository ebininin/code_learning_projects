class Employee:

    num_of_emps = 0
    raise_amount = 1.04

    def __init__(self, first, last, pay):
        self.first = first
        self.last = last
        self.pay = pay
        self.email = f'{first}.{last}@company.com'

        Employee.num_of_emps += 1

    def fullname(self):
        return f'{self.first} {self.last}'

    def appy_raise(self):
        self.pay = int(self.pay * self.raise_amount)

    @classmethod
    def set_rates_amt(cls, amount):
        cls.raise_amount = amount

    @classmethod
    def from_string(cls, emp_str):
        first, last, pay = emp_str.split('-')
        cls(first, last, pay)

    @staticmethod
    def is_workday(day):
        if day.weekday() == 5 or day.weekday() == 6:
            return False
        return True

    def __repr__(self):
        return f"Empoyee(f'{self.first}, {self.last}, {self.pay})"

    def __str__(self):
        return f'{self.fullname()} - {self.email}'

    def __add__(self, other):
        return self.pay + other.pay

    def __len__(self):
        return len(self.fullname())


class Developer(Employee):
    raise_amount = 1.2

    def __init__(self, first, last, pay, prog_lang):
        super().__init__(first, last, pay)
        self.prog_lang = prog_lang


class Manager(Employee):

    def __init__(self, first, last, pay, employees=None):
        super().__init__(first, last, pay)
        if employees is None:
            self.employees = []
        else:
            self.employees = employees

    def add_emp(self, emp):
        if emp not in self.employees:
            self.employees.append(emp)

    def remove_emp(self, emp):
        if emp in self.employees:
            self.employees.remove(emp)

    def print_emps(self):
        for emp in self.employees:
            print('---->', emp.fullname())


emp_1 = Employee('coy', 'sadcan', 55434)
emp_2 = Employee('corey', 'scan', 5434)

dev_1 = Developer('Corel', 'Cia', 55434, 'JAV')

mgr_1 = Manager('Susan', 'Saa', 9000, [dev_1, emp_2])

# mgr_1.print_emps()
#
# print(isinstance(mgr_1, Employee))
# print(dev_1)
# print(dev_1.__str__())
# print(dev_1.__repr__())
print(emp_1 + emp_2)
print(Employee)

