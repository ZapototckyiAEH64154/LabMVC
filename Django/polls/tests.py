import datetime

from django.test import TestCase
from django.utils import timezone
from django.urls import reverse

from .models import Question


def create_question(question_text, days):
    """
    Create a question with the given `question_text` and published the
    given number of `days` offset to now (negative for past, positive for future).
    """
    time = timezone.now() + datetime.timedelta(days=days)
    return Question.objects.create(
        question_text=question_text,
        pub_date=time
    )


class QuestionModelTests(TestCase):

    def test_was_published_recently_with_future_question(self):
        """
        Returns False for questions with pub_date in the future
        """
        time = timezone.now() + datetime.timedelta(days=30)
        future_question = Question(pub_date=time)

        self.assertIs(future_question.was_published_recently(), False)

    def test_was_published_recently_with_old_question(self):
        """
        Returns False for questions older than 1 day
        """
        time = timezone.now() - datetime.timedelta(days=1, seconds=1)
        old_question = Question(pub_date=time)

        self.assertIs(old_question.was_published_recently(), False)

    def test_was_published_recently_with_recent_question(self):
        """
        Returns True for questions within the last day
        """
        time = timezone.now() - datetime.timedelta(hours=23, minutes=59, seconds=59)
        recent_question = Question(pub_date=time)

        self.assertIs(recent_question.was_published_recently(), True)


class QuestionIndexViewTests(TestCase):

    def test_no_questions(self):
        """
        If no questions exist, show message
        """
        response = self.client.get(reverse("polls:index"))

        self.assertEqual(response.status_code, 200)
        self.assertContains(response, "No polls are available.")
        self.assertQuerySetEqual(response.context["latest_question_list"], [])

    def test_past_question(self):
        """
        Past questions should appear
        """
        question = create_question("Past question.", -30)

        response = self.client.get(reverse("polls:index"))

        self.assertQuerySetEqual(
            response.context["latest_question_list"],
            [question],
        )

    def test_future_question(self):
        """
        Future questions should NOT appear
        """
        create_question("Future question.", 30)

        response = self.client.get(reverse("polls:index"))

        self.assertContains(response, "No polls are available.")
        self.assertQuerySetEqual(response.context["latest_question_list"], [])

    def test_future_question_and_past_question(self):
        """
        Only past questions appear if both exist
        """
        question = create_question("Past question.", -30)
        create_question("Future question.", 30)

        response = self.client.get(reverse("polls:index"))

        self.assertQuerySetEqual(
            response.context["latest_question_list"],
            [question],
        )

    def test_two_past_questions(self):
        """
        Multiple past questions should appear
        """
        question1 = create_question("Past question 1.", -30)
        question2 = create_question("Past question 2.", -5)

        response = self.client.get(reverse("polls:index"))

        self.assertQuerySetEqual(
            response.context["latest_question_list"],
            [question2, question1],
        )


class QuestionDetailViewTests(TestCase):

    def test_future_question(self):
        """
        Future question should return 404
        """
        future_question = create_question("Future question.", 5)

        url = reverse("polls:detail", args=(future_question.id,))
        response = self.client.get(url)

        self.assertEqual(response.status_code, 404)

    def test_past_question(self):
        """
        Past question should be visible
        """
        past_question = create_question("Past question.", -5)

        url = reverse("polls:detail", args=(past_question.id,))
        response = self.client.get(url)

        self.assertContains(response, past_question.question_text)