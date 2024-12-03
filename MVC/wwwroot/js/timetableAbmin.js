
// $(document).ready(function () {
//     let data;
//     $.ajax({
//         url: 'http://localhost:5064/api/Teacher/timetable',
//         method: 'GET',
//         dataType: 'json',
//         success: function (responseData) {
//             data = responseData;
//             console.log(data);

//             // Initialize the DropDownList with data fetched from the API
//             $.ajax({
//                 url: 'http://localhost:5064/api/Teacher/Getstandards', // Replace with your API endpoint
//                 method: 'GET',
//                 dataType: 'json',
//                 success: function (response) {
//                     // Assuming response is an array of standards, e.g., [1, 2, 3, 4, 5]
//                     $("#standardSelect").kendoDropDownList({
//                         optionLabel: "Select Standard",
//                         filter: "contains",
//                         dataTextField: "text",
//                         dataValueField: "value",
//                         dataSource: response.map(standard => ({
//                             text: `Standard ${standard}`,
//                             value: `Standard ${standard}`
//                         })),
//                         change: function (e) {
//                             const selectedStandard = this.value();
//                             loadSchedulerData(selectedStandard); // Load scheduler data on change
//                         }
//                     });
//                 },
//                 error: function () {
//                     console.error("Failed to load standards data.");
//                 }
//             });

//             const teacherAssignments = {};

//             // Prepare teacher assignments once timetables data is available
//             data.timetables.forEach(({ c_standard, c_subject_name, c_name }) => {
//                 const standardKey = `Standard ${c_standard}`;
//                 if (!teacherAssignments[standardKey]) {
//                     teacherAssignments[standardKey] = {};
//                 }
//                 teacherAssignments[standardKey][c_subject_name] = c_name;
//             });

//             // // Initialize the scheduler with the loaded data
//             // const startDate = new Date("2024-01-01");
//             // const generateTimeTable = (standard) => {
//             //     let idCounter = 1;
//             //     const TimeTable = [];
//             //     const subjects = Object.keys(teacherAssignments[standard] || {});
//             //     const assignedTeachers = {}; // To keep track of teachers assigned to timeslots

//             //     const days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"];
//             //     const times = [
//             //         { label: "9:00 AM - 10:00 AM", start: "09:00", end: "10:00" },
//             //         { label: "10:00 AM - 11:00 AM", start: "10:00", end: "11:00" },
//             //         { label: "11:00 AM - 12:00 PM", start: "11:00", end: "12:00" },
//             //         { label: "12:00 PM - 1:00 PM", start: "12:00", end: "13:00", subject: "Lunch Break" },
//             //         { label: "1:00 PM - 2:00 PM", start: "13:00", end: "14:00" },
//             //         { label: "2:00 PM - 3:00 PM", start: "14:00", end: "15:00" },
//             //         { label: "3:00 PM - 4:00 PM", start: "15:00", end: "16:00" },
//             //         { label: "4:00 PM - 5:00 PM", start: "16:00", end: "17:00" },
//             //     ];

//             //     days.forEach((day, dayIndex) => {
//             //         const currentDay = new Date(startDate);
//             //         currentDay.setDate(startDate.getDate() + dayIndex);

//             //         let subjectIndex = 0;
//             //         const assignedSubjects = new Set();

//             //         times.forEach((time) => {
//             //             // Handle the lunch break case
//             //             if (time.subject === "Lunch Break") {
//             //                 TimeTable.push({
//             //                     id: idCounter++,
//             //                     day: day,
//             //                     start: new Date(currentDay.toISOString().split('T')[0] + "T" + time.start + ":00"),
//             //                     end: new Date(currentDay.toISOString().split('T')[0] + "T" + time.end + ":00"),
//             //                     title: "Lunch Break",
//             //                     teacher: "N/A",
//             //                     std: standard
//             //                 });
//             //                 return; // Skip the rest of the code for lunch break
//             //             }

//             //             let title = "No Lecture";
//             //             let teacher = "N/A";

//             //             if (day + '-' + time.label == "Monday-9:00 AM - 10:00 AM") {
//             //                 console.log(assignedSubjects);


//             //             }
//             //             // Assign subject and teacher
//             //             if (subjectIndex < subjects.length && !assignedSubjects.has(subjects[subjectIndex])) {
//             //                 const subject = subjects[subjectIndex];
//             //                 teacher = teacherAssignments[standard][subject];
//             //                 title = `${subject} by ${teacher}`;
//             //                 assignedSubjects.add(subject);
//             //                 subjectIndex++;
//             //             }

//             //             if (day + '-' + time.label == "Monday-9:00 AM - 10:00 AM") {
//             //                 console.log(assignedTeachers);
//             //                 console.log(teacher);
//             //                 console.log(day + '-' + time.label)
//             //             }



//             //             // Ensure no conflicts: Check if teacher is already assigned at this time
//             //             if (assignedTeachers[`${day}-${time.label}`] && assignedTeachers[`${day}-${time.label}`].includes(teacher)) {
//             //                 // If the teacher is assigned, move to the next subject
//             //                 let newSubjectIndex = (subjectIndex + 1) % subjects.length;
//             //                 let newSubject = subjects[newSubjectIndex];
//             //                 let newTeacher = teacherAssignments[standard][newSubject];

//             //                 // Update to the new subject and teacher if necessary
//             //                 title = `${newSubject} by ${newTeacher}`;
//             //                 teacher = newTeacher;
//             //                 assignedSubjects.add(newSubject);
//             //                 subjectIndex = newSubjectIndex;

//             //                 console.log('confilt=' + teacher);
//             //             }

//             //             TimeTable.push({
//             //                 id: idCounter++,
//             //                 day: day,
//             //                 start: new Date(currentDay.toISOString().split('T')[0] + "T" + time.start + ":00"),
//             //                 end: new Date(currentDay.toISOString().split('T')[0] + "T" + time.end + ":00"),
//             //                 title: title,
//             //                 teacher: teacher,
//             //                 std: standard
//             //             });

//             //             // Track the teacher assigned to the specific time slot
//             //             if (!assignedTeachers[`${day}-${time.label}`]) {
//             //                 assignedTeachers[`${day}-${time.label}`] = [];
//             //             }
//             //             assignedTeachers[`${day}-${time.label}`].push(teacher);
//             //         });
//             //     });

//             //     console.log(TimeTable);
//             //     return TimeTable;
//             // };


//             const startDate = new Date("2024-01-01");
//             const assignedTeachers = {};

//             // const generateTimeTable = (standard) => {
//             //     let idCounter = 1;
//             //     const TimeTable = [];
//             //     const subjects = Object.keys(teacherAssignments[standard] || {});

//             //     const days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"];
//             //     const times = [
//             //         { label: "9:00 AM - 10:00 AM", start: "09:00", end: "10:00" },
//             //         { label: "10:00 AM - 11:00 AM", start: "10:00", end: "11:00" },
//             //         { label: "11:00 AM - 12:00 PM", start: "11:00", end: "12:00" },
//             //         { label: "12:00 PM - 1:00 PM", start: "12:00", end: "13:00", subject: "Lunch Break" },
//             //         { label: "1:00 PM - 2:00 PM", start: "13:00", end: "14:00" },
//             //         { label: "2:00 PM - 3:00 PM", start: "14:00", end: "15:00" },
//             //         { label: "3:00 PM - 4:00 PM", start: "15:00", end: "16:00" },
//             //         { label: "4:00 PM - 5:00 PM", start: "16:00", end: "17:00" },
//             //     ];

//             //     days.forEach((day, dayIndex) => {
//             //         const currentDay = new Date(startDate);
//             //         currentDay.setDate(startDate.getDate() + dayIndex);

//             //         let subjectIndex = 0;
//             //         const assignedSubjects = new Set();

//             //         times.forEach((time) => {
//             //             if (time.subject === "Lunch Break") {
//             //                 TimeTable.push({
//             //                     id: idCounter++,
//             //                     day: day,
//             //                     start: new Date(currentDay.toISOString().split('T')[0] + "T" + time.start + ":00"),
//             //                     end: new Date(currentDay.toISOString().split('T')[0] + "T" + time.end + ":00"),
//             //                     title: "Lunch Break",
//             //                     teacher: "N/A",
//             //                     std: standard
//             //                 });
//             //                 return;
//             //             }

//             //             let title = "No Lecture";
//             //             let teacher = "N/A";

//             //             // Assign subject and teacher only if no conflict
//             //             while (subjectIndex < subjects.length && !assignedSubjects.has(subjects[subjectIndex])) {
//             //                 const subject = subjects[subjectIndex];
//             //                 teacher = teacherAssignments[standard][subject];

//             //                 // Check for conflicts across standards
//             //                 if (!assignedTeachers[`${day}-${time.label}`] || !assignedTeachers[`${day}-${time.label}`].includes(teacher)) {
//             //                     title = `${subject} by ${teacher}`;
//             //                     assignedSubjects.add(subject);

//             //                     if (!assignedTeachers[`${day}-${time.label}`]) {
//             //                         assignedTeachers[`${day}-${time.label}`] = [];
//             //                     }
//             //                     assignedTeachers[`${day}-${time.label}`].push(teacher);
//             //                     subjectIndex++;
//             //                     break;
//             //                 } else {
//             //                     subjectIndex++;
//             //                 }
//             //             }

//             //             TimeTable.push({
//             //                 id: idCounter++,
//             //                 day: day,
//             //                 start: new Date(currentDay.toISOString().split('T')[0] + "T" + time.start + ":00"),
//             //                 end: new Date(currentDay.toISOString().split('T')[0] + "T" + time.end + ":00"),
//             //                 title: title,
//             //                 teacher: teacher,
//             //                 std: standard
//             //             });
//             //         });
//             //     });

//             //     return TimeTable;
//             // };


//             const generateTimeTable = (standard) => {
//                 let idCounter = 1;
//                 const TimeTable = [];
//                 const subjects = Object.keys(teacherAssignments[standard] || {});

//                 const days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"];
//                 const times = [
//                     { label: "9:00 AM - 10:00 AM", start: "09:00", end: "10:00" },
//                     { label: "10:00 AM - 11:00 AM", start: "10:00", end: "11:00" },
//                     { label: "11:00 AM - 12:00 PM", start: "11:00", end: "12:00" },
//                     { label: "12:00 PM - 1:00 PM", start: "12:00", end: "13:00", subject: "Lunch Break" },
//                     { label: "1:00 PM - 2:00 PM", start: "13:00", end: "14:00" },
//                     { label: "2:00 PM - 3:00 PM", start: "14:00", end: "15:00" },
//                     { label: "3:00 PM - 4:00 PM", start: "15:00", end: "16:00" },
//                     { label: "4:00 PM - 5:00 PM", start: "16:00", end: "17:00" },
//                 ];

//                 days.forEach((day, dayIndex) => {
//                     const currentDay = new Date(startDate);
//                     currentDay.setDate(startDate.getDate() + dayIndex);

//                     let subjectIndex = 0;
//                     const assignedSubjects = new Set();

//                     times.forEach((time) => {
//                         let title = "No Lecture";
//                         let teacher = "N/A";

//                         // Handle the lunch break case directly
//                         if (time.subject === "Lunch Break") {
//                             TimeTable.push({
//                                 id: idCounter++,
//                                 day: day,
//                                 start: new Date(currentDay.toISOString().split('T')[0] + "T" + time.start + ":00"),
//                                 end: new Date(currentDay.toISOString().split('T')[0] + "T" + time.end + ":00"),
//                                 title: "Lunch Break",
//                                 teacher: "N/A",
//                                 std: standard
//                             });
//                             return;
//                         }

//                         // Subject and teacher assignment logic
//                         let attempts = 0;  // To prevent infinite loops
//                         while (subjectIndex < subjects.length && attempts < subjects.length) {
//                             const subject = subjects[subjectIndex];
//                             teacher = teacherAssignments[standard][subject];

//                             // Ensure teacher isn’t assigned elsewhere at this time
//                             if (!assignedTeachers[`${day}-${time.label}`] || !assignedTeachers[`${day}-${time.label}`].includes(teacher)) {
//                                 title = `${subject} by ${teacher}`;
//                                 assignedSubjects.add(subject);

//                                 if (!assignedTeachers[`${day}-${time.label}`]) {
//                                     assignedTeachers[`${day}-${time.label}`] = [];
//                                 }
//                                 assignedTeachers[`${day}-${time.label}`].push(teacher);
//                                 subjectIndex++;
//                                 break;  // Exit the loop if successful assignment
//                             } else {
//                                 // Conflict found; try next subject
//                                 subjectIndex = (subjectIndex + 1) % subjects.length;
//                                 attempts++;
//                             }
//                         }

//                         // Populate the timetable slot, even if 'No Lecture'
//                         TimeTable.push({
//                             id: idCounter++,
//                             day: day,
//                             start: new Date(currentDay.toISOString().split('T')[0] + "T" + time.start + ":00"),
//                             end: new Date(currentDay.toISOString().split('T')[0] + "T" + time.end + ":00"),
//                             title: title,
//                             teacher: teacher,
//                             std: standard
//                         });
//                     });
//                 });

//                 console.log(TimeTable);
//                 return TimeTable;
//             };



//             const loadSchedulerData = (selectedStandard) => {
//                 const scheduler = $("#scheduler").data("kendoScheduler");
//                 const filteredData = generateTimeTable(selectedStandard);
//                 scheduler.dataSource.data(filteredData);
//             };

//             // Initialize the Kendo Scheduler
//             const standardColors = {
//                 "Standard 1": "#cc3333",
//                 "Standard 2": "#33cc33",
//                 "Standard 3": "#3333cc",
//                 "Standard 4": "#cccc33",
//                 "Standard 5": "#cc9933",
//                 "Standard 6": "#5798be",
//                 "Standard 7": "#3333cc",
//                 "Standard 8": "#3333cc",

//             };

//             $("#scheduler").kendoScheduler({
//                 date: new Date("2024-01-01"),
//                 startTime: new Date("2024-01-01T09:00:00"),
//                 endTime: new Date("2024-01-01T17:00:00"),
//                 height: 800,
//                 views: [
//                     { type: "day", selected: true },
//                     { type: "week" }
//                 ],
//                 dataSource: new kendo.data.SchedulerDataSource({
//                     data: [],
//                     schema: {
//                         model: {
//                             id: "id",
//                             fields: {
//                                 id: { type: "number" },
//                                 title: { type: "string", validation: { required: true } },
//                                 start: { type: "date", validation: { required: true } },
//                                 end: { type: "date", validation: { required: true } },
//                                 std: { type: "string" },
//                                 teacher: { type: "string" }
//                             }
//                         }
//                     }
//                 }),
//                 eventTemplate: function (event) {
//                     return `<div style="background-color:${standardColors[event.std]}; padding: 4px;">${event.title}</div>`;
//                 }
//             });

//             // Initial load for Standard 1
//             loadSchedulerData("Standard 1");
//         },
//         error: function () {
//             console.error("Failed to load data.");
//         }
//     });
// });

// $(document).ready(function () {
//     let data;
//     $.ajax({
//         url: 'http://localhost:5064/api/Teacher/timetable',
//         method: 'GET',
//         dataType: 'json',
//         success: function (responseData) {
//             data = responseData;
//             console.log(data);

//             // Initialize the DropDownList with data fetched from the API
//             $.ajax({
//                 url: 'http://localhost:5064/api/Teacher/Getstandards', // Replace with your API endpoint
//                 method: 'GET',
//                 dataType: 'json',
//                 success: function (response) {
//                     // Assuming response is an array of standards, e.g., [1, 2, 3, 4, 5]
//                     $("#standardSelect").kendoDropDownList({
//                         optionLabel: "Select Standard",
//                         filter: "contains",
//                         dataTextField: "text",
//                         dataValueField: "value",
//                         dataSource: response.map(standard => ({
//                             text: `Standard ${standard}`,
//                             value: `Standard ${standard}`
//                         })),
//                         change: function (e) {
//                             const selectedStandard = this.value();
//                             loadSchedulerData(selectedStandard); // Load scheduler data on change
//                         }
//                     });
//                 },
//                 error: function () {
//                     console.error("Failed to load standards data.");
//                 }
//             });

//             const teacherAssignments = {};
//             const assignedTeachers = {}; // Reset for each standard to avoid conflict

//             // Prepare teacher assignments once timetables data is available
//             data.timetables.forEach(({ c_standard, c_subject_name, c_name }) => {
//                 const standardKey = `Standard ${c_standard}`;
//                 if (!teacherAssignments[standardKey]) {
//                     teacherAssignments[standardKey] = {};
//                 }
//                 teacherAssignments[standardKey][c_subject_name] = c_name;
//             });

//             const startDate = new Date("2024-01-01");

//             const generateTimeTable = (standard) => {
//                 let idCounter = 1;
//                 const TimeTable = [];
//                 const subjects = Object.keys(teacherAssignments[standard] || {});

//                 const days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"];
//                 const times = [
//                     { label: "9:00 AM - 10:00 AM", start: "09:00", end: "10:00" },
//                     { label: "10:00 AM - 11:00 AM", start: "10:00", end: "11:00" },
//                     { label: "11:00 AM - 12:00 PM", start: "11:00", end: "12:00" },
//                     { label: "12:00 PM - 1:00 PM", start: "12:00", end: "13:00", subject: "Lunch Break" },
//                     { label: "1:00 PM - 2:00 PM", start: "13:00", end: "14:00" },
//                     { label: "2:00 PM - 3:00 PM", start: "14:00", end: "15:00" },
//                     { label: "3:00 PM - 4:00 PM", start: "15:00", end: "16:00" },
//                     { label: "4:00 PM - 5:00 PM", start: "16:00", end: "17:00" },
//                 ];



//                 days.forEach((day, dayIndex) => {
//                     const currentDay = new Date(startDate);
//                     currentDay.setDate(startDate.getDate() + dayIndex);

//                     let subjectIndex = 0;
//                     const assignedSubjects = new Set();

//                     times.forEach((time) => {
//                         let title = "No Lecture";
//                         let teacher = "N/A";

//                         // Handle the lunch break case directly
//                         if (time.subject === "Lunch Break") {
//                             TimeTable.push({
//                                 id: idCounter++,
//                                 day: day,
//                                 start: new Date(currentDay.toISOString().split('T')[0] + "T" + time.start + ":00"),
//                                 end: new Date(currentDay.toISOString().split('T')[0] + "T" + time.end + ":00"),
//                                 title: "Lunch Break",
//                                 teacher: "N/A",
//                                 std: standard
//                             });
//                             return;
//                         }

//                         // Subject and teacher assignment logic
//                         let attempts = 0;  // To prevent infinite loops
//                         while (subjectIndex < subjects.length && attempts < subjects.length) {
//                             const subject = subjects[subjectIndex];
//                             teacher = teacherAssignments[standard][subject];

//                             // Ensure teacher isn’t assigned elsewhere at this time
//                             const timeSlotKey = `${day}-${time.label}`;
//                             if (!assignedTeachers[timeSlotKey] || !assignedTeachers[timeSlotKey].includes(teacher)) {
//                                 title = `${subject} by ${teacher}`;
//                                 assignedSubjects.add(subject);

//                                 // Track teacher assignment for this time slot
//                                 if (!assignedTeachers[timeSlotKey]) {
//                                     assignedTeachers[timeSlotKey] = [];
//                                 }
//                                 assignedTeachers[timeSlotKey].push(teacher);
//                                 subjectIndex++;
//                                 break;  // Exit the loop if successful assignment
//                             } else {
//                                 // Conflict found; try next subject
//                                 subjectIndex = (subjectIndex + 1) % subjects.length;
//                                 attempts++;
//                             }
//                         }

//                         // Populate the timetable slot, even if 'No Lecture'
//                         TimeTable.push({
//                             id: idCounter++,
//                             day: day,
//                             start: new Date(currentDay.toISOString().split('T')[0] + "T" + time.start + ":00"),
//                             end: new Date(currentDay.toISOString().split('T')[0] + "T" + time.end + ":00"),
//                             title: title,
//                             teacher: teacher,
//                             std: standard
//                         });
//                     });
//                 });

//                 console.log(TimeTable);
//                 return TimeTable;
//             };

//             const loadSchedulerData = (selectedStandard) => {
//                 const scheduler = $("#scheduler").data("kendoScheduler");
//                 const filteredData = generateTimeTable(selectedStandard);
//                 scheduler.dataSource.data(filteredData);
//             };

//             // Initialize the Kendo Scheduler
//             const standardColors = {
//                 "Standard 1": "#cc3333",
//                 "Standard 2": "#33cc33",
//                 "Standard 3": "#3333cc",
//                 "Standard 4": "#cccc33",
//                 "Standard 5": "#cc9933",
//                 "Standard 6": "#5798be",
//                 "Standard 7": "#3333cc",
//                 "Standard 8": "#3333cc",
//             };

//             $("#scheduler").kendoScheduler({
//                 date: new Date("2024-01-01"),
//                 startTime: new Date("2024-01-01T09:00:00"),
//                 endTime: new Date("2024-01-01T17:00:00"),
//                 height: 800,
//                 views: [
//                     { type: "day", selected: true },
//                     { type: "week" }
//                 ],
//                 dataSource: new kendo.data.SchedulerDataSource({
//                     data: [],
//                     schema: {
//                         model: {
//                             id: "id",
//                             fields: {
//                                 id: { type: "number" },
//                                 title: { type: "string", validation: { required: true } },
//                                 start: { type: "date", validation: { required: true } },
//                                 end: { type: "date", validation: { required: true } },
//                                 std: { type: "string" },
//                                 teacher: { type: "string" }
//                             }
//                         }
//                     }
//                 }),
//                 eventTemplate: function (event) {
//                     return `<div style="background-color:${standardColors[event.std]}; padding: 4px;">${event.title}</div>`;
//                 }
//             });

//             // Initial load for Standard 1
//             loadSchedulerData("Standard 1");
//         },
//         error: function () {
//             console.error("Failed to load data.");
//         }
//     });
// });


$(document).ready(function () {
    let data;

    // Fetch timetable data
    $.ajax({
        url: 'http://localhost:5064/api/Teacher/timetable',
        method: 'GET',
        dataType: 'json',
        success: function (responseData) {
            data = responseData;
            console.log(data);

            // Initialize the standard dropdown
            $.ajax({
                url: 'http://localhost:5064/api/Teacher/Getstandards',
                method: 'GET',
                dataType: 'json',
                success: function (response) {
                    // Populate dropdown with standards
                    $("#standardSelect").kendoDropDownList({
                        optionLabel: "Select Standard",
                        filter: "contains",
                        dataTextField: "text",
                        dataValueField: "value",
                        dataSource: response.map(standard => ({
                            text: `Standard ${standard}`,
                            value: `Standard ${standard}`
                        })),
                        change: function (e) {
                            const selectedStandard = this.value();
                            loadSchedulerData(selectedStandard); // Filter timetable and update scheduler
                        }
                    });
                },
                error: function () {
                    console.error("Failed to load standards data.");
                }
            });

            const teacherAssignments = {};
           

            // Prepare teacher assignments once timetable data is available
            data.timetables.forEach(({ c_standard, c_subject_name, c_name }) => {
                const standardKey = `Standard ${c_standard}`;
                if (!teacherAssignments[standardKey]) {
                    teacherAssignments[standardKey] = {};
                }
                teacherAssignments[standardKey][c_subject_name] = c_name;
            });
            const startDate = new Date("2024-01-01");
            const allTimeTables = [];  // Array to hold the timetable for all standards
            const assignedTeachers = {};  // Track teachers' assignments
            
            const generateTimeTableForAllStandards = () => {
                const standards = Object.keys(teacherAssignments);  // Assuming teacherAssignments contains all standards
                const days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"];
                const times = [
                    { label: "9:00 AM - 10:00 AM", start: "09:00", end: "10:00" },
                    { label: "10:00 AM - 11:00 AM", start: "10:00", end: "11:00" },
                    { label: "11:00 AM - 12:00 PM", start: "11:00", end: "12:00" },
                    { label: "12:00 PM - 1:00 PM", start: "12:00", end: "13:00", subject: "Lunch Break" },
                    { label: "1:00 PM - 2:00 PM", start: "13:00", end: "14:00" },
                    { label: "2:00 PM - 3:00 PM", start: "14:00", end: "15:00" },
                    { label: "3:00 PM - 4:00 PM", start: "15:00", end: "16:00" },
                    { label: "4:00 PM - 5:00 PM", start: "16:00", end: "17:00" },
                ];
            
                standards.forEach((standard) => {
                    let idCounter = 1;
                    const TimeTable = [];
                    const subjects = Object.keys(teacherAssignments[standard] || {});
            
                    days.forEach((day, dayIndex) => {
                        const currentDay = new Date(startDate);
                        currentDay.setDate(startDate.getDate() + dayIndex);
            
                        const assignedSubjects = new Set();  // Track assigned subjects for the day
            
                        times.forEach((time, timeIndex) => {
                            let title = "No Lecture";
                            let teacher = "N/A";
            
                            // Handle the lunch break case directly
                            if (time.subject === "Lunch Break") {
                                TimeTable.push({
                                    id: idCounter++,
                                    day: day,
                                    start: new Date(currentDay.toISOString().split('T')[0] + "T" + time.start + ":00"),
                                    end: new Date(currentDay.toISOString().split('T')[0] + "T" + time.end + ":00"),
                                    title: "Lunch Break",
                                    teacher: "N/A",
                                    std: standard
                                });
                                return;
                            }
            
                            // Attempt to assign a subject and teacher if no prior assignment is made
                            for (let subjectIndex = 0; subjectIndex < subjects.length; subjectIndex++) {
                                const subject = subjects[subjectIndex];
                                teacher = teacherAssignments[standard][subject];
                                const timeSlotKey = `${day}-${time.label}`;
                                let conflictFound = false;
            
                                // Check for conflicts across all standards for the same teacher at this time
                                for (const std in assignedTeachers) {
                                    if (assignedTeachers[std][timeSlotKey] && assignedTeachers[std][timeSlotKey].includes(teacher)) {
                                        conflictFound = true;
                                        break;
                                    }
                                }
            
                                // If no conflict is found and the subject has not been assigned already, proceed
                                if (!conflictFound && !assignedSubjects.has(subject)) {
                                    title = `${subject} by ${teacher}`;
                                    assignedSubjects.add(subject);
            
                                    // Track the teacher's assignment for this time slot
                                    if (!assignedTeachers[standard]) {
                                        assignedTeachers[standard] = {};
                                    }
                                    if (!assignedTeachers[standard][timeSlotKey]) {
                                        assignedTeachers[standard][timeSlotKey] = [];
                                    }
                                    assignedTeachers[standard][timeSlotKey].push(teacher);
            
                                    break;  // Exit the loop if assignment is successful
                                }
                            }
            
                            // If no assignment is possible, retain "No Lecture"
                            TimeTable.push({
                                id: idCounter++,
                                day: day,
                                start: new Date(currentDay.toISOString().split('T')[0] + "T" + time.start + ":00"),
                                end: new Date(currentDay.toISOString().split('T')[0] + "T" + time.end + ":00"),
                                title: title,
                                teacher: teacher,
                                std: standard
                            });
                        });
                    });
            
                    // Add the generated timetable for the current standard to the allTimeTables array
                    allTimeTables.push({ standard, timeTable: TimeTable });
                });
            
                console.log(allTimeTables);  // All timetables for all standards
                return allTimeTables;
            };
            
            generateTimeTableForAllStandards();
            
            
            

            const loadSchedulerData = (selectedStandard) => {
                const scheduler = $("#scheduler").data("kendoScheduler");
            
                // Filter timetable data by the selected standard
                const filteredData = allTimeTables.find(item => item.standard === selectedStandard)?.timeTable || [];
            
                // Validate the structure of the data before updating the Scheduler
                filteredData.forEach(item => {
                    console.log(item);
                    if (!item.start || !item.end || !item.title) {
                        console.error("Invalid data:", item);
                    }
                });
            
                // Update the scheduler data source
                scheduler.dataSource.data(filteredData);
            };
            

            // Initialize the Kendo Scheduler
            const standardColors = {
                "Standard 1": "#cc3333",
                "Standard 2": "#33cc33",
                "Standard 3": "#3333cc",
                "Standard 4": "#cccc33",
                "Standard 5": "#cc9933",
                "Standard 6": "#5798be",
                "Standard 7": "#3333cc",
                "Standard 8": "#3333cc",
            };

            $("#scheduler").kendoScheduler({
                date: new Date("2024-01-01"),
                startTime: new Date("2024-01-01T09:00:00"),
                endTime: new Date("2024-01-01T17:00:00"),
                height: 800,
                views: [
                    { type: "day", selected: true },
                    { type: "week" }
                ],
                dataSource: new kendo.data.SchedulerDataSource({
                    data: [],
                    schema: {
                        model: {
                            id: "id",
                            fields: {
                                id: { type: "number" },
                                title: { type: "string", validation: { required: true } },
                                start: { type: "date", validation: { required: true } },
                                end: { type: "date", validation: { required: true } },
                                std: { type: "string" },
                                teacher: { type: "string" }
                            }
                        }
                    }
                    
                }),
                eventTemplate: function (event) {
                    return `<div style="background-color:${standardColors[event.std]}; padding: 4px;">${event.title}</div>`;
                }
            });

            // Initial load for Standard 1
            loadSchedulerData("Standard 1");
        },
        error: function () {
            console.error("Failed to load data.");
        }
    });
});

