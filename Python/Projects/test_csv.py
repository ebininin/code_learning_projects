import csv

with open("txt_file/test_csv.csv", "r") as csv_file:
    csv_reader = csv.DictReader(csv_file)

    with open("txt_file/test_csv_parse.csv", "w") as new_file:
        fieldnames = ["Name", "Age", "Position"]
        csv_writer = csv.DictWriter(new_file, fieldnames=fieldnames, delimiter="\t")

        csv_writer.writeheader()

        for record in csv_reader:
            del record["Salary"]
            csv_writer.writerow(record)

# with open("txt_file/test_csv.csv", "r") as csv_file:
#     csv_reader = csv.reader(csv_file)

    # with open("txt_file/test_csv_parse.csv", "w") as new_file:
    #     csv_writer = csv.writer(new_file, delimiter="\t")
    #
    #     for record in csv_reader:
    #         csv_writer.writerow(record)
