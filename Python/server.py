from flask import Flask, send_file
import matplotlib.pyplot as plt
import io
import plotly.express as px
from flask import Flask, jsonify
from flask_cors import CORS
import matplotlib

matplotlib.use('Agg')
app = Flask(__name__)
CORS(app)

@app.route('/subjects')
def subjects():
   
    subjects = ['Math', 'Science', 'English', 'History']
    subjects_student_count = [50, 60, 40, 30]
    subjects_title = "Students Enrolled under Subject"
    
    subjects_fig, subjects_ax = plt.subplots()
    subjects_ax.bar(subjects, subjects_student_count)
    subjects_ax.set_xlabel('Subjects')
    subjects_ax.set_ylabel('Number of Students')
    subjects_ax.set_title(subjects_title)

    subjects_img = io.BytesIO()
    plt.savefig(subjects_img, format='png')
    subjects_img.seek(0)
    plt.close(subjects_fig)

    return send_file(subjects_img, mimetype='image/png')

@app.route('/sports')
def sports():
    sports = ['Football', 'Cricket', 'Volleyball', 'Handball']
    sports_student_count = [50, 40, 35, 25]
    sports_title = "Students Interested in Sports"
      
    sports_fig, sports_ax = plt.subplots()
    sports_ax.bar(sports, sports_student_count)
    sports_ax.set_xlabel('Sports')
    sports_ax.set_ylabel('Number of Students')
    sports_ax.set_title(sports_title)

    sports_img = io.BytesIO()
    plt.savefig(sports_img, format='png')
    sports_img.seek(0)
    plt.close(sports_fig)

    return send_file(sports_img, mimetype='image/png')

@app.route('/teachers')
def teachers():
    teachers = ['Yash', 'Anvi', 'Dhruv', 'Ayush']
    teachers_student_count = [45, 55, 35, 50]
    teachers_title = "Students under Counselling of Teachers"
    
    teachers_fig, teachers_ax = plt.subplots()
    teachers_ax.bar(teachers, teachers_student_count)
    teachers_ax.set_xlabel('Teachers')
    teachers_ax.set_ylabel('Number of Students')
    teachers_ax.set_title(teachers_title)

    teachers_img = io.BytesIO()
    plt.savefig(teachers_img, format='png')
    teachers_img.seek(0)
    plt.close(teachers_fig)

    return send_file(teachers_img, mimetype='image/png')

@app.route('/courses')
def courses():
    courses = ['Engineering', 'Medical', 'Law', 'Architecture']
    courses_student_count = [60, 50, 40, 45]
    courses_title = "Students interested in Professional Courses"
   
    courses_fig, courses_ax = plt.subplots()
    courses_ax.bar(courses, courses_student_count)
    courses_ax.set_xlabel('Teachers')
    courses_ax.set_ylabel('Number of Students')
    courses_ax.set_title(courses_title)

    courses_img = io.BytesIO()
    plt.savefig(courses_img, format='png')
    courses_img.seek(0)
    plt.close(courses_fig)

    return send_file(courses_img, mimetype='image/png')

@app.route('/generate-chart')
def generate_chart():
    subjects = ['Math', 'Science', 'English', 'History']
    student_count = [50, 60, 40, 30]

    fig, ax = plt.subplots()
    ax.bar(subjects, student_count)

    ax.set_xlabel('Subjects')
    ax.set_ylabel('Number of Students')
    ax.set_title('Student Enrollment by Subject')

    img = io.BytesIO()
    plt.savefig(img, format='png')
    img.seek(0)

    return send_file(img, mimetype='image/png')

@app.route('/generate-interactive-chart')
def generate_interactive_chart():
    subjects = ['Math', 'Science', 'English', 'History']
    student_count = [50, 60, 40, 30]

    fig = px.bar(x=subjects, y=student_count, labels={'x': 'Subjects', 'y': 'Number of Students'}, title="Student Enrollment by Subject")

    fig.update_layout(
        title={
            'text': "Student Enrollment by Subject",
            'x': 0.5,
            'xanchor': 'center',
            'y': 0.95,
            'yanchor': 'top',
            'font': {
                'size': 20,
                'family': "Arial, sans-serif",
                'color': "darkblue"
            }
        },
        xaxis_title="Subjects",
        yaxis_title="Number of Students",
        plot_bgcolor='rgba(240, 240, 240, 0.95)',
        paper_bgcolor='white',
        font=dict(
            family="Arial, sans-serif",
            size=12,
            color="black"
        ),
        margin=dict(l=40, r=40, t=40, b=40)
    )

    return fig.to_html(full_html=False)

if __name__ == '__main__':
    app.run(debug=True)