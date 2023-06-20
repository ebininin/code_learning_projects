import csv

html_output = ""
names = []

with open("txt_file/text_csv_real.csv", "r") as data_file:
    csv_data = csv.DictReader(data_file)

    #  remove headers of bad data
    next(csv_data)

    for line in csv_data:
        if line["Name"] == "NoReward":
            break
        names.append(f"{line['Name']}")

html_output += f"<p>There are currently {len(names)} contributors.</p>"
html_output += "\n<ul>"

for name in names:
    html_output += f"\n\t<li>{name}</li>"

html_output += "\n</ul>"

print(html_output)
