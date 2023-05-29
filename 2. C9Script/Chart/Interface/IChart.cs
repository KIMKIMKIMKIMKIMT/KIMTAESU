using LitJson;

public interface IChart<out T>
{
    T GetID();
    void SetData(JsonData jsonData);
}