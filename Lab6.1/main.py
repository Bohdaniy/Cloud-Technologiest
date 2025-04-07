import requests
import json
from datetime import datetime


class NERApplication:
    def __init__(self, endpoint, key):
        self.endpoint = endpoint
        self.key = key
        self.headers = {
            "Ocp-Apim-Subscription-Key": key,
            "Content-Type": "application/json",
            "Accept": "application/json"
        }

    def recognize_entities(self, text):
        """Виділяє іменовані сутності з тексту"""
        data = {
            "kind": "EntityRecognition",
            "parameters": {
                "modelVersion": "latest"
            },
            "analysisInput": {
                "documents": [
                    {
                        "id": "1",
                        "language": "uk",
                        "text": text
                    }
                ]
            }
        }

        response = requests.post(
            f"{self.endpoint}/language/:analyze-text?api-version=2023-04-01",
            headers=self.headers,
            json=data
        )

        if response.status_code == 200:
            return response.json()
        else:
            raise Exception(f"Помилка: {response.status_code} - {response.text}")

    def print_results(self, result):
        """Виводить результати аналізу у зручному форматі"""
        print("\nРезультати аналізу тексту:")
        print("=" * 50)

        for document in result["results"]["documents"]:
            print(f"\nДокумент ID: {document['id']}")
            print("-" * 50)

            for entity in document["entities"]:
                print(f"\nСутність: {entity['text']}")
                print(f"Категорія: {entity['category']}")
                print(f"Підкатегорія: {entity.get('subcategory', 'немає')}")
                print(f"Впевненість: {entity['confidenceScore']:.2f}")
                if 'offset' in entity:
                    print(f"Позиція: символи {entity['offset']}-{entity['offset'] + entity['length']}")

    def run(self):
        """Головний цикл програми"""
        print("Аналізатор іменованих сутностей (NER)")
        print("=" * 50)
        print(f"Використовується кінцева точка: {self.endpoint}")
        print(f"Дата: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
        print("\nВведіть текст для аналізу (або 'exit' для виходу):")

        while True:
            text = input("\n> ")
            if text.lower() == 'exit':
                break

            try:
                result = self.recognize_entities(text)
                self.print_results(result)
            except Exception as e:
                print(f"Сталася помилка: {str(e)}")


# Конфігурація (замініть на свої значення)
ENDPOINT = "https://labitoss5.cognitiveservices.azure.com/"
KEY = "9hhl7bcPKgyfrDoIPP9agktDkDI70judyfkeqGgAha5tfrfaEnjZJQQJ99BCACYeBjFXJ3w3AAAaACOGy03I"

if __name__ == "__main__":
    try:
        app = NERApplication(ENDPOINT, KEY)
        app.run()
    except KeyboardInterrupt:
        print("\nРоботу програми перервано")
    except Exception as e:
        print(f"Критична помилка: {str(e)}")