-- Inserting Math Questions
INSERT INTO temp_questions (question_text, difficulty_level, category, hint, correct_answer, answer_option1, answer_option2, answer_option3, answer_option4)
VALUES
('What is 2 + 2?', 'easy', 'Math', 'Basic arithmetic', '4', '3', '4', '5', '6'),
('What is 3 * 3?', 'easy', 'Math', 'Multiplication', '9', '6', '7', '9', '10'),
('What is the square root of 16?', 'medium', 'Math', 'Square root calculation', '4', '2', '3', '4', '5'),
('Solve for x: 2x + 3 = 7', 'medium', 'Math', 'Simple algebra', '2', '1', '2', '3', '4'),
-- Add 96 more Math questions here
('What is the integral of 2x dx?', 'hard', 'Math', 'Integration', 'x^2 + C', 'x + C', 'x^2 + C', '2x + C', 'x^2/2 + C');

-- Inserting Geography Questions
INSERT INTO temp_questions (question_text, difficulty_level, category, hint, correct_answer, answer_option1, answer_option2, answer_option3, answer_option4)
VALUES
('What is the capital of France?', 'easy', 'Geography', 'Capital of a European country', 'Paris', 'London', 'Berlin', 'Paris', 'Rome'),
('Which continent is the Sahara Desert located on?', 'easy', 'Geography', 'Largest desert in the world', 'Africa', 'Asia', 'Africa', 'Australia', 'North America'),
('Which country has the longest coastline?', 'medium', 'Geography', 'Think about the largest countries', 'Canada', 'Russia', 'Australia', 'Canada', 'USA'),
('In which country is the Amazon rainforest located?', 'medium', 'Geography', 'Largest rainforest in the world', 'Brazil', 'Brazil', 'Peru', 'Colombia', 'Venezuela'),
-- Add 96 more Geography questions here
('What is the highest mountain in the world?', 'hard', 'Geography', 'It is located in Asia', 'Mount Everest', 'K2', 'Mount Everest', 'Kangchenjunga', 'Lhotse');

-- Inserting History Questions
INSERT INTO temp_questions (question_text, difficulty_level, category, hint, correct_answer, answer_option1, answer_option2, answer_option3, answer_option4)
VALUES
('Who was the first President of the United States?', 'easy', 'History', 'American Revolution leader', 'George Washington', 'Thomas Jefferson', 'Abraham Lincoln', 'George Washington', 'John Adams'),
('In which year did World War I begin?', 'easy', 'History', 'Early 20th century', '1914', '1912', '1914', '1916', '1918'),
('Who was the first man to step on the moon?', 'medium', 'History', 'Apollo 11 mission', 'Neil Armstrong', 'Buzz Aldrin', 'Neil Armstrong', 'Michael Collins', 'Yuri Gagarin'),
('What was the ancient Egyptian writing system called?', 'medium', 'History', 'Used in pyramids and tombs', 'Hieroglyphics', 'Hieroglyphics', 'Cuneiform', 'Latin', 'Sanskrit'),
-- Add 96 more History questions here
('Which empire was known as the "Land of the Rising Sun"?', 'hard', 'History', 'An East Asian empire', 'Japan', 'China', 'Japan', 'Korea', 'Vietnam');

-- Inserting Science Questions
INSERT INTO temp_questions (question_text, difficulty_level, category, hint, correct_answer, answer_option1, answer_option2, answer_option3, answer_option4)
VALUES
('What is the chemical symbol for water?', 'easy', 'Science', 'Two hydrogen atoms and one oxygen atom', 'H2O', 'H2', 'H2O', 'O2', 'HO2'),
('What planet is known as the Red Planet?', 'easy', 'Science', 'Fourth planet from the Sun', 'Mars', 'Venus', 'Mars', 'Jupiter', 'Saturn'),
('What is the speed of light?', 'medium', 'Science', 'Measured in meters per second', '299,792,458 m/s', '299,792 m/s', '299,792,458 m/s', '300,000,000 m/s', '150,000,000 m/s'),
('What is the powerhouse of the cell?', 'medium', 'Science', 'Produces energy', 'Mitochondria', 'Nucleus', 'Mitochondria', 'Ribosome', 'Chloroplast'),
-- Add 96 more Science questions here
('What is the most abundant gas in the Earth’s atmosphere?', 'hard', 'Science', 'Makes up around 78%', 'Nitrogen', 'Oxygen', 'Nitrogen', 'Carbon Dioxide', 'Argon');
