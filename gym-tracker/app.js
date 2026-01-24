// 90 Days Gym Journey Tracker - JavaScript

// Workout schedule for each day of the week
const weeklySchedule = {
    monday: {
        type: 'legs',
        name: 'LEGS - Quad Focus',
        exercises: [
            'Barbell Squats - 4x8-10',
            'Leg Press - 4x12',
            'Walking Lunges - 3x12 each',
            'Leg Extensions - 3x15',
            'Calf Raises - 4x15'
        ]
    },
    tuesday: {
        type: 'push',
        name: 'PUSH - Chest & Shoulders',
        exercises: [
            'Bench Press - 4x8-10',
            'Incline Dumbbell Press - 4x10',
            'Overhead Press - 4x8',
            'Lateral Raises - 4x15',
            'Tricep Dips - 3x12'
        ]
    },
    wednesday: {
        type: 'pull',
        name: 'PULL - Back & Biceps',
        exercises: [
            'Deadlifts - 4x6-8',
            'Pull-Ups/Lat Pulldown - 4x10',
            'Barbell Rows - 4x10',
            'Face Pulls - 3x15',
            'Barbell Curls - 3x12'
        ]
    },
    thursday: {
        type: 'legs',
        name: 'LEGS - Hamstring & Glutes',
        exercises: [
            'Romanian Deadlifts - 4x10',
            'Bulgarian Split Squats - 3x10 each',
            'Leg Curls - 4x12',
            'Hip Thrusts - 4x12',
            'Standing Calf Raises - 4x15'
        ]
    },
    friday: {
        type: 'upper',
        name: 'UPPER - Hypertrophy',
        exercises: [
            'Incline Bench Press - 4x10',
            'Seated Cable Rows - 4x12',
            'Arnold Press - 3x12',
            'Cable Flyes - 3x15',
            'Hammer Curls - 3x12'
        ]
    },
    saturday: {
        type: 'rest',
        name: 'ACTIVE RECOVERY',
        exercises: [
            'Light Cardio - 20-30 min',
            'Stretching & Mobility',
            'Foam Rolling',
            'Optional: Swimming/Walking'
        ]
    },
    sunday: {
        type: 'rest',
        name: 'REST DAY',
        exercises: [
            'Complete Rest',
            'Meal Prep',
            'Recovery & Sleep',
            'Plan Next Week'
        ]
    }
};

// Day type mapping for 90 days (based on day of week starting from start date)
function getDayType(dayNumber, startDate) {
    const date = new Date(startDate);
    date.setDate(date.getDate() + dayNumber - 1);
    const dayOfWeek = date.getDay();
    const days = ['sunday', 'monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday'];
    return weeklySchedule[days[dayOfWeek]].type;
}

// Initialize app
class GymTracker {
    constructor() {
        this.startDate = this.getStartDate();
        this.completedDays = this.getCompletedDays();
        this.measurements = this.getMeasurements();
        this.currentPhase = 1;

        this.init();
    }

    init() {
        this.updateStats();
        this.renderCalendar(1);
        this.renderTodayWorkout();
        this.setupEventListeners();
        this.loadMeasurements();
        this.updateStartDate();
    }

    // Local Storage Methods
    getStartDate() {
        let startDate = localStorage.getItem('gymJourney_startDate');
        if (!startDate) {
            startDate = new Date().toISOString().split('T')[0];
            localStorage.setItem('gymJourney_startDate', startDate);
        }
        return startDate;
    }

    getCompletedDays() {
        const saved = localStorage.getItem('gymJourney_completedDays');
        return saved ? JSON.parse(saved) : [];
    }

    saveCompletedDays() {
        localStorage.setItem('gymJourney_completedDays', JSON.stringify(this.completedDays));
    }

    getMeasurements() {
        const saved = localStorage.getItem('gymJourney_measurements');
        return saved ? JSON.parse(saved) : {};
    }

    saveMeasurements() {
        localStorage.setItem('gymJourney_measurements', JSON.stringify(this.measurements));
    }

    // Calculate current day of journey
    getCurrentDay() {
        const start = new Date(this.startDate);
        const today = new Date();
        const diffTime = Math.abs(today - start);
        const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
        return Math.min(diffDays + 1, 90);
    }

    // Update all statistics
    updateStats() {
        const currentDay = this.getCurrentDay();
        const completedCount = this.completedDays.length;
        const progress = Math.round((completedCount / 90) * 100);
        const streak = this.calculateStreak();
        const remaining = 90 - currentDay + 1;

        document.getElementById('currentDay').textContent = currentDay;
        document.getElementById('completedWorkouts').textContent = completedCount;
        document.getElementById('progressPercent').textContent = progress + '%';
        document.getElementById('currentStreak').textContent = streak;
        document.getElementById('daysRemaining').textContent = remaining + ' days remaining';
        document.getElementById('progressFill').style.width = progress + '%';

        // Update phase indicators
        this.updatePhaseIndicators(currentDay);
    }

    calculateStreak() {
        if (this.completedDays.length === 0) return 0;

        let streak = 0;
        const today = this.getCurrentDay();

        for (let i = today; i >= 1; i--) {
            if (this.completedDays.includes(i)) {
                streak++;
            } else {
                break;
            }
        }
        return streak;
    }

    updatePhaseIndicators(currentDay) {
        const phases = document.querySelectorAll('.phase');
        phases.forEach((phase, index) => {
            phase.classList.remove('active');
            if (index === 0 && currentDay <= 30) phase.classList.add('active');
            else if (index === 1 && currentDay > 30 && currentDay <= 60) phase.classList.add('active');
            else if (index === 2 && currentDay > 60) phase.classList.add('active');
        });
    }

    // Render 90-day calendar
    renderCalendar(phase) {
        const grid = document.getElementById('calendarGrid');
        grid.innerHTML = '';

        const startDay = (phase - 1) * 30 + 1;
        const endDay = phase * 30;
        const currentDay = this.getCurrentDay();

        for (let day = startDay; day <= endDay; day++) {
            const dayElement = document.createElement('div');
            dayElement.className = 'calendar-day';
            dayElement.dataset.day = day;

            const dayType = getDayType(day, this.startDate);

            // Add day type class
            if (dayType !== 'rest') {
                dayElement.classList.add(dayType + '-day');
            } else {
                dayElement.classList.add('rest');
            }

            // Check if completed
            if (this.completedDays.includes(day)) {
                dayElement.classList.add('completed');
            }

            // Check if today
            if (day === currentDay) {
                dayElement.classList.add('today');
            }

            dayElement.innerHTML = `
                <span class="day-num">${day}</span>
                <span class="day-type">${dayType.charAt(0).toUpperCase()}</span>
            `;

            dayElement.addEventListener('click', () => this.toggleDay(day));
            grid.appendChild(dayElement);
        }
    }

    // Toggle day completion
    toggleDay(day) {
        const currentDay = this.getCurrentDay();

        // Only allow toggling current day or past days
        if (day > currentDay) {
            alert('Cannot mark future days as complete!');
            return;
        }

        const index = this.completedDays.indexOf(day);
        if (index > -1) {
            this.completedDays.splice(index, 1);
        } else {
            this.completedDays.push(day);
        }

        this.saveCompletedDays();
        this.renderCalendar(this.currentPhase);
        this.updateStats();
    }

    // Render today's workout
    renderTodayWorkout() {
        const workoutLog = document.getElementById('workoutLog');
        const today = new Date();
        const days = ['sunday', 'monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday'];
        const dayName = days[today.getDay()];
        const workout = weeklySchedule[dayName];

        workoutLog.innerHTML = `
            <div class="workout-header">
                <h3 style="color: var(--primary); margin-bottom: 15px;">${workout.name}</h3>
            </div>
        `;

        workout.exercises.forEach((exercise, index) => {
            const exerciseId = `exercise-${index}`;
            const isChecked = this.isExerciseChecked(exerciseId);

            const item = document.createElement('div');
            item.className = `exercise-item ${isChecked ? 'checked' : ''}`;
            item.innerHTML = `
                <input type="checkbox" id="${exerciseId}" ${isChecked ? 'checked' : ''}>
                <label for="${exerciseId}">${exercise}</label>
            `;

            item.querySelector('input').addEventListener('change', (e) => {
                this.toggleExercise(exerciseId, e.target.checked);
                item.classList.toggle('checked', e.target.checked);
            });

            workoutLog.appendChild(item);
        });

        // Update complete button state
        this.updateCompleteButton();
    }

    isExerciseChecked(exerciseId) {
        const todayKey = new Date().toISOString().split('T')[0];
        const todayExercises = JSON.parse(localStorage.getItem(`gymJourney_exercises_${todayKey}`) || '[]');
        return todayExercises.includes(exerciseId);
    }

    toggleExercise(exerciseId, checked) {
        const todayKey = new Date().toISOString().split('T')[0];
        let todayExercises = JSON.parse(localStorage.getItem(`gymJourney_exercises_${todayKey}`) || '[]');

        if (checked && !todayExercises.includes(exerciseId)) {
            todayExercises.push(exerciseId);
        } else if (!checked) {
            todayExercises = todayExercises.filter(e => e !== exerciseId);
        }

        localStorage.setItem(`gymJourney_exercises_${todayKey}`, JSON.stringify(todayExercises));
        this.updateCompleteButton();
    }

    updateCompleteButton() {
        const currentDay = this.getCurrentDay();
        const btn = document.getElementById('completeWorkout');

        if (this.completedDays.includes(currentDay)) {
            btn.textContent = 'Completed ✓';
            btn.disabled = true;
        } else {
            btn.textContent = 'Mark Today Complete ✓';
            btn.disabled = false;
        }
    }

    // Setup event listeners
    setupEventListeners() {
        // Phase buttons
        document.querySelectorAll('.phase-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                document.querySelectorAll('.phase-btn').forEach(b => b.classList.remove('active'));
                e.target.classList.add('active');
                this.currentPhase = parseInt(e.target.dataset.phase);
                this.renderCalendar(this.currentPhase);
            });
        });

        // Complete workout button
        document.getElementById('completeWorkout').addEventListener('click', () => {
            const currentDay = this.getCurrentDay();
            if (!this.completedDays.includes(currentDay)) {
                this.completedDays.push(currentDay);
                this.saveCompletedDays();
                this.updateStats();
                this.renderCalendar(this.currentPhase);
                this.updateCompleteButton();
                this.showCelebration();
            }
        });

        // Save measurements button
        document.getElementById('saveMeasurements').addEventListener('click', () => {
            this.saveMeasurementsForm();
        });
    }

    // Load measurements into form
    loadMeasurements() {
        const fields = ['weight', 'chest', 'waist', 'thighs', 'arms', 'shoulders'];
        const latestDate = Object.keys(this.measurements).sort().pop();

        if (latestDate && this.measurements[latestDate]) {
            fields.forEach(field => {
                const input = document.getElementById(field);
                if (input && this.measurements[latestDate][field]) {
                    input.value = this.measurements[latestDate][field];
                }
            });
        }
    }

    // Save measurements from form
    saveMeasurementsForm() {
        const fields = ['weight', 'chest', 'waist', 'thighs', 'arms', 'shoulders'];
        const today = new Date().toISOString().split('T')[0];

        this.measurements[today] = {};

        fields.forEach(field => {
            const input = document.getElementById(field);
            if (input && input.value) {
                this.measurements[today][field] = parseFloat(input.value);
            }
        });

        this.saveMeasurements();
        alert('Measurements saved successfully!');
    }

    // Update start date display
    updateStartDate() {
        const startDateEl = document.getElementById('startDate');
        const date = new Date(this.startDate);
        startDateEl.textContent = date.toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'long',
            day: 'numeric'
        });
    }

    // Show celebration animation
    showCelebration() {
        const celebration = document.createElement('div');
        celebration.innerHTML = '🎉 Great Job! Keep Going! 💪';
        celebration.style.cssText = `
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            background: linear-gradient(135deg, #6366f1, #8b5cf6);
            color: white;
            padding: 30px 50px;
            border-radius: 20px;
            font-size: 1.5rem;
            font-weight: bold;
            z-index: 1000;
            animation: celebrationPop 0.5s ease;
            box-shadow: 0 20px 60px rgba(99, 102, 241, 0.5);
        `;

        document.body.appendChild(celebration);

        setTimeout(() => {
            celebration.style.animation = 'celebrationFade 0.3s ease';
            setTimeout(() => celebration.remove(), 300);
        }, 2000);
    }
}

// Add celebration animations
const style = document.createElement('style');
style.textContent = `
    @keyframes celebrationPop {
        0% { transform: translate(-50%, -50%) scale(0); opacity: 0; }
        50% { transform: translate(-50%, -50%) scale(1.1); }
        100% { transform: translate(-50%, -50%) scale(1); opacity: 1; }
    }
    @keyframes celebrationFade {
        to { opacity: 0; transform: translate(-50%, -50%) scale(0.8); }
    }
`;
document.head.appendChild(style);

// Initialize the app when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    window.gymTracker = new GymTracker();
});

// Service Worker Registration for offline support (optional)
if ('serviceWorker' in navigator) {
    window.addEventListener('load', () => {
        // navigator.serviceWorker.register('/sw.js')
        //     .then(reg => console.log('Service Worker registered'))
        //     .catch(err => console.log('Service Worker registration failed'));
    });
}
