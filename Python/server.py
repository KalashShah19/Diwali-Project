import psycopg2
import plotly.graph_objects as go
from flask import Flask
import plotly.express as px
from flask import Flask
from flask_cors import CORS

app = Flask(__name__)
CORS(app)

@app.route('/')
def index():
    return "<h1>Chart Gallery</h1><ul>" + "".join([f"<li><h2><a href='/{chart}'>{chart}</a></h2></li>" for chart in endpoints]) + "</ul>"

DB_CONFIG = {
    'dbname': 'School',
    'user': 'postgres',
    'password': '123456',
    'host': 'cipg01',
    'port': 5432
}

@app.route('/gender-distribution')
def gender_distribution():
    try:
        conn = psycopg2.connect(**DB_CONFIG)
        cursor = conn.cursor()

        query = """
        SELECT c_role, c_gender, COUNT(*) 
        FROM t_users 
        WHERE c_role IN ('Teacher', 'Student') 
        GROUP BY c_role, c_gender 
        ORDER BY c_role, c_gender;
        """
        cursor.execute(query)
        data = cursor.fetchall()

        cursor.close()
        conn.close()

        roles = ["Teacher", "Student"]
        male = [0, 0]
        female = [0, 0]

        for row in data:
            role, gender, count = row
            if role == "Teacher" and gender == "Male":
                male[0] = count
            elif role == "Teacher" and gender == "Female":
                female[0] = count
            elif role == "Student" and gender == "Male":
                male[1] = count
            elif role == "Student" and gender == "Female":
                female[1] = count

        fig = go.Figure()
        fig.add_trace(go.Bar(x=roles, y=male, name='Male', marker_color='blue'))
        fig.add_trace(go.Bar(x=roles, y=female, name='Female', marker_color='#ec2ecd'))
        fig.update_layout(
            title="Gender Distribution",
            barmode='group',
            xaxis_title="Role",
            yaxis_title="Count",
            xaxis=dict(tickangle=45),
            template="plotly_white"
        )

        return fig.to_html(full_html=False)
    except Exception as e:
        return f"Error: {str(e)}"

@app.route('/age-distribution')
def age_distribution():
    try:
        conn = psycopg2.connect(**DB_CONFIG)
        cursor = conn.cursor()

        query = """
        SELECT 
            CASE
                WHEN EXTRACT(YEAR FROM AGE(CURRENT_DATE, c_birth_date)) BETWEEN 10 AND 15 THEN '10-15'
                WHEN EXTRACT(YEAR FROM AGE(CURRENT_DATE, c_birth_date)) BETWEEN 16 AND 20 THEN '16-20'
                WHEN EXTRACT(YEAR FROM AGE(CURRENT_DATE, c_birth_date)) BETWEEN 21 AND 25 THEN '21-25'
                ELSE 'Older'
            END AS age_group,
            COUNT(*) AS count
        FROM t_users
        WHERE c_role = 'Student'
        GROUP BY age_group
        ORDER BY age_group;
        """
        cursor.execute(query)
        data = cursor.fetchall()

        cursor.close()
        conn.close()

        age_groups = []
        counts = []
        for row in data:
            age_groups.append(row[0])
            counts.append(row[1])

        fig = px.bar(
            x=age_groups,
            y=counts,
            labels={"x": "Age Group", "y": "Count"},
            title="Age Distribution of Students",
            color_discrete_sequence=px.colors.qualitative.Set2
        )

        return fig.to_html(full_html=False)
    except Exception as e:
        return f"Error: {str(e)}"
    
@app.route('/age-distribution-teachers')
def age_distribution_teachers():
    try:
        conn = psycopg2.connect(**DB_CONFIG)
        cursor = conn.cursor()

        query = """
        SELECT 
            CASE
               WHEN EXTRACT(YEAR FROM AGE(CURRENT_DATE, c_birth_date)) BETWEEN 20 AND 25 THEN '20-25'
                WHEN EXTRACT(YEAR FROM AGE(CURRENT_DATE, c_birth_date)) BETWEEN 26 AND 30 THEN '26-30'
                WHEN EXTRACT(YEAR FROM AGE(CURRENT_DATE, c_birth_date)) BETWEEN 31 AND 35 THEN '31-35'
                WHEN EXTRACT(YEAR FROM AGE(CURRENT_DATE, c_birth_date)) BETWEEN 36 AND 40 THEN '36-40'
                WHEN EXTRACT(YEAR FROM AGE(CURRENT_DATE, c_birth_date)) BETWEEN 40 AND 45 THEN '40-45'
                WHEN EXTRACT(YEAR FROM AGE(CURRENT_DATE, c_birth_date)) BETWEEN 45 AND 50 THEN '45-50'
                WHEN EXTRACT(YEAR FROM AGE(CURRENT_DATE, c_birth_date)) BETWEEN 55 AND 60 THEN '55-60'
                WHEN EXTRACT(YEAR FROM AGE(CURRENT_DATE, c_birth_date)) BETWEEN 60 AND 65 THEN '60-65'
                WHEN EXTRACT(YEAR FROM AGE(CURRENT_DATE, c_birth_date)) BETWEEN 70 AND 75 THEN '70-75'
            END AS age_group,
            COUNT(*) AS count
        FROM t_users
        WHERE c_role = 'Teacher'
        GROUP BY age_group
        ORDER BY age_group;
        """
        cursor.execute(query)
        data = cursor.fetchall()

        cursor.close()
        conn.close()

        age_groups = []
        counts = []
        for row in data:
            age_groups.append(row[0])
            counts.append(row[1])

        fig = px.bar(
            x=age_groups,
            y=counts,
            labels={"x": "Age Group", "y": "Count"},
            title="Age Distribution of Teachers",
            color_discrete_sequence=px.colors.qualitative.Set2
        )

        return fig.to_html(full_html=False)
    except Exception as e:
        return f"Error: {str(e)}"
    
@app.route('/studying-vs-left-students')
def studying_vs_left_students():
    try:
        # Connect to PostgreSQL
        conn = psycopg2.connect(**DB_CONFIG)
        cursor = conn.cursor()

        # SQL query to count students studying vs those left
        query = """
        SELECT
            COUNT(CASE WHEN t_students.c_studying = TRUE THEN 1 END) AS studying,
            COUNT(CASE WHEN t_students.c_studying = FALSE THEN 1 END) AS left
        FROM t_students;
        """
        cursor.execute(query)
        data = cursor.fetchone()

        # Close the connection
        cursor.close()
        conn.close()

        # Prepare data for the pie chart
        studying_count = data[0]
        left_count = data[1]
        labels = ["Studying", "Left"]
        values = [studying_count, left_count]
        
        # Pie chart colors
        colors = ["green", "red"]

        # Generate the pie chart
        fig = px.pie(
            values=values,
            names=labels,
            title="Studying vs Left Students",
            color_discrete_sequence=colors
        )
        fig.update_traces(textinfo="percent+label", pull=[0.1, 0])

        return fig.to_html(full_html=False)
    except Exception as e:
        return f"Error: {str(e)}"

@app.route('/teachers-by-standard')
def teachers_by_standard():
    try:
        # Connect to PostgreSQL
        conn = psycopg2.connect(**DB_CONFIG)
        cursor = conn.cursor()

        # SQL query to count teachers by standard
        query = """
        SELECT t_classwise_subjects.c_standard, COUNT(t_teachers.c_teacher_id) AS teacher_count
        FROM t_classwise_subjects
        JOIN t_teachers ON t_classwise_subjects.c_teacher_id = t_teachers.c_teacher_id
        GROUP BY t_classwise_subjects.c_standard Order by c_standard;
        """
        cursor.execute(query)
        data = cursor.fetchall()

        # Close the connection
        cursor.close()
        conn.close()

        # Prepare data for the bar chart
        standards = [row[0] for row in data]  # Extract standards
        teacher_counts = [row[1] for row in data]  # Extract teacher counts

        # Generate the bar chart
        fig = px.bar(
            x=standards,
            y=teacher_counts,
            labels={"x": "Standard", "y": "Teachers"},
            title="Teachers by Standard"
        )

        return fig.to_html(full_html=False)
    except Exception as e:
        return f"Error: {str(e)}"


@app.route('/teacher-education-levels')
def teacher_education_levels():
    try:
        # Connect to PostgreSQL
        conn = psycopg2.connect(**DB_CONFIG)
        cursor = conn.cursor()

        # SQL query to get teacher education levels
        query = """
        SELECT c_qualification, COUNT(*) 
        FROM t_teachers
        GROUP BY c_qualification;
        """
        cursor.execute(query)
        data = cursor.fetchall()

        # Close the connection
        cursor.close()
        conn.close()

        # Prepare data for the pie chart
        education_levels = [row[0] for row in data]  # Extract education levels
        counts = [row[1] for row in data]

        # Generate the pie chart
        fig = px.pie(
            values=counts,
            names=education_levels,
            title="Teacher Education Levels"
        )

        return fig.to_html(full_html=False)
    except Exception as e:
        return f"Error: {str(e)}"

@app.route('/fees-by-standard')
def fees_by_standard():
    try:
        # Connect to PostgreSQL
        conn = psycopg2.connect(**DB_CONFIG)
        cursor = conn.cursor()

        # SQL query to get fees by standard
        query = """
        SELECT s.c_standard, SUM(f.c_amount) as total_fees
        FROM t_fees_structure f
        JOIN t_standards s ON f.c_standard = s.c_standard
        GROUP BY s.c_standard;
        """
        cursor.execute(query)
        data = cursor.fetchall()

        # Close the connection
        cursor.close()
        conn.close()

        # Prepare data for the bar chart
        standards = [row[0] for row in data]  # Extract standards
        fees = [row[1] for row in data]  # Extract total fees for each standard

        # Generate the bar chart
        fig = px.bar(
            x=standards,
            y=fees,
            labels={"x": "Standard", "y": "Fees"},
            title="Fees by Standard"
        )

        return fig.to_html(full_html=False)
    except Exception as e:
        return f"Error: {str(e)}"

@app.route('/yearly-revenue')
def yearly_revenue():
    try:
        conn = psycopg2.connect(**DB_CONFIG)
        cursor = conn.cursor()

        query = """
            SELECT
            fs.c_batch_year AS year,
            SUM(fs.c_Amount) AS total_revenue
        FROM
            t_Fees_structure fs
        JOIN
            t_Payments p
            ON fs.c_id = p.c_id
        WHERE
            p.c_status = 'Completed'
        GROUP BY
            fs.c_batch_year
        ORDER BY
            fs.c_batch_year;
        """
        cursor.execute(query)
        data = cursor.fetchall()

        # Close the connection
        cursor.close()
        conn.close()

        # Prepare data for the line chart
        years = [str(int(row[0])) for row in data]  # Extract and format the year
        revenue = [row[1] for row in data]  # Extract total revenue for each year
        print(revenue)

        # Generate the line chart with green color
        fig = px.line(
            x=years,
            y=revenue,
            labels={"x": "Year", "y": "Revenue"},
            title="Yearly Revenue",
            line_shape="linear"  # Optional: line shape
        )
        
        # Update line color to green
        fig.update_traces(line=dict(color='green'))

        return fig.to_html(full_html=False)
    except Exception as e:
        return f"Error: {str(e)}"


@app.route('/syllabus-status')
def syllabus_status():
    try:
        # Connect to PostgreSQL
        conn = psycopg2.connect(**DB_CONFIG)
        cursor = conn.cursor()

        # SQL query to count completed and pending syllabus chapters based on completion percentage
        query = """
             SELECT 
            COUNT(CASE WHEN c_completed >= .5 THEN 1 END) AS completed,
            COUNT(CASE WHEN c_completed < .5 THEN 1 END) AS pending
        FROM 
		t_Syllabus
        """
        cursor.execute(query)
        data = cursor.fetchone()

        # Close the connection
        cursor.close()
        conn.close()

        # Extract data for pie chart
        completed = data[0]  # Number of completed syllabus chapters
        pending = data[1]  # Number of pending syllabus chapters

        # Prepare data for the pie chart
        labels = ["Completed", "Pending"]
        values = [completed, pending]

        # Custom contrasting colors for the pie chart
        colors = ["#4CAF50", "#FF5733"]  # Green for Completed, Red for Pending

        # Generate the pie chart
        fig = px.pie(
            values=values,
            names=labels,
            title="Syllabus Status",
            color_discrete_sequence=colors
        )

        # Optionally, add a slight pull effect to highlight the "Completed" section
        fig.update_traces(textinfo="percent+label", pull=[0.1, 0])

        return fig.to_html(full_html=False)

    except Exception as e:
        return f"Error: {str(e)}"


@app.route('/feedback-ratings')
def feedback_ratings():
    try:
        # Connect to PostgreSQL
        conn = psycopg2.connect(**DB_CONFIG)
        cursor = conn.cursor()

        # SQL query to get the count of feedback ratings (1 to 5 stars)
        query = """
             SELECT 
                c_rating, 
                COUNT(*) 
            FROM 
                t_Feedbacks
            GROUP BY 
                c_rating
            ORDER BY 
                c_rating;
        """
        cursor.execute(query)
        data = cursor.fetchall()

        # Close the connection
        cursor.close()
        conn.close()

        # Prepare data for the pie chart
        ratings = [f"{row[0]} Star" for row in data]  # Format ratings as "1 Star", "2 Stars", etc.
        counts = [row[1] for row in data]  # Extract the count of each rating

        # Generate the pie chart
        fig = px.pie(
            values=counts,
            names=ratings,
            title="Feedback Ratings"
        )

        # Optionally, customize colors if needed
        # fig.update_traces(marker=dict(colors=['#ff6347', '#ffa07a', '#f0e68c', '#98fb98', '#2e8b57']))

        return fig.to_html(full_html=False)

    except Exception as e:
        return f"Error: {str(e)}"


endpoints = [
    "gender-distribution", "age-distribution", "age-distribution-teachers",
    "studying-vs-left-students", "teachers-by-standard", "teacher-education-levels", "fees-by-standard", 
    "yearly-revenue", "syllabus-status", "feedback-ratings"
]

if __name__ == '__main__':
    app.run(debug=True)