namespace HopFrame.Web.Admin.Generators;

public interface IGenerator<out TGeneratedType> {

    TGeneratedType Compile();

}