# class Employee:
#
#     def __init__(self, first, last):
#         self.first = first
#         self.last = last
#
#     @property
#     def email(self):
#         return f'{self.first}.{self.last}@email.com'
#
#     @property
#     def fullname(self):
#         return f'{self.first} {self.last}'
#
#     @fullname.setter
#     def fullname(self, name):
#         first, last = name.split(' ')
#         self.first = first
#         self.last = last
#
#     @fullname.deleter
#     def fullname(self):
#         print('Delete Name: ')
#         self.first = None
#         self.last = None
#
# emp_1 = Employee('Claude', 'Vagan')
#
# emp_1.fullname = 'Gim Fia'
#
# print(emp_1.first)
# print(emp_1.last)
# print(emp_1.fullname)
# print(emp_1.email)
#
# del emp_1.fullname


import collections

lst = ["Free", "Code", "Camp", "Code", "Free"]

print(collections.Counter(lst))