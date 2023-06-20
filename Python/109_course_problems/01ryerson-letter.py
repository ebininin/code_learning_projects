def ryerson_letter_grade(score):
    score_dict = [
        [90, 101, 'A+'],
        [85, 90, 'A'],
        [80, 85, 'A-'],
        [77, 80, 'B+'],
        [73, 77, 'B'],
        [70, 73, 'B-'],
        [67, 70, 'C+'],
        [63, 67, 'C'],
        [60, 63, 'C-'],
        [57, 60, 'D+'],
        [53, 57, 'D'],
        [50, 53, 'D-'],
        [0, 50, 'F']
    ]

    for grade in score_dict:
        if score in range(grade[0], grade[1]):
            print(grade[2])


ryerson_letter_grade(1)
